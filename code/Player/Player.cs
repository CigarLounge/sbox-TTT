using Sandbox;

namespace TTT;

[Title( "Player" ), Icon( "emoji_people" )]
public partial class Player : AnimatedEntity
{
	public Inventory Inventory { get; private init; }
	public Perks Perks { get; private init; }

	public CameraMode Camera
	{
		get => Components.Get<CameraMode>();
		set
		{
			var current = Camera;
			if ( current == value )
				return;

			Components.RemoveAny<CameraMode>();
			Components.Add( value );
		}
	}

	/// <summary>
	/// The player earns score by killing players on opposite teams, confirming bodies
	/// or surviving the round.
	/// </summary>
	public int Score
	{
		get => Client.GetInt( Strings.Score );
		set => Client.SetInt( Strings.Score, value );
	}

	/// <summary>
	/// The score gained during a single round. This gets added to the actual score
	/// at the end of a round.
	/// </summary>
	public int RoundScore { get; set; }

	public const float DropVelocity = 300;

	public Player()
	{
		Inventory = new( this );
		Perks = new( this );
	}

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "player" );
		SetModel( "models/citizen/citizen.vmdl" );
		Role = new NoneRole();

		Health = 0;
		LifeState = LifeState.Respawnable;
		Transmit = TransmitType.Always;

		Predictable = true;
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableLagCompensation = true;
		EnableShadowInFirstPerson = true;

		Animator = new PlayerAnimator();
		Camera = new FreeSpectateCamera();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Role = new NoneRole();
	}

	public void Respawn()
	{
		Host.AssertServer();

		LifeState = LifeState.Respawnable;
		IsSpectator = IsForcedSpectator;

		DeleteFlashlight();
		DeleteItems();
		ResetConfirmationData();
		Role = new NoneRole();

		TimeUntilClean = 0;
		Velocity = Vector3.Zero;
		WaterLevel = 0;
		Credits = 0;

		if ( !IsForcedSpectator )
		{
			Health = MaxHealth;
			LifeState = LifeState.Alive;

			EnableAllCollisions = true;
			EnableDrawing = true;

			Controller = new WalkController();
			Camera = new FirstPersonCamera();

			CreateHull();
			CreateFlashlight();
			DressPlayer();
			ResetInterpolation();

			Event.Run( TTTEvent.Player.Spawned, this );
			Game.Current.State.OnPlayerSpawned( this );
		}
		else
		{
			MakeSpectator( false );
		}

		ClientRespawn( this );
	}

	private void ClientRespawn()
	{
		Host.AssertClient();

		ResetConfirmationData();
		DeleteFlashlight();

		if ( !IsLocalPawn )
			Role = new NoneRole();
		else
			ClearButtons();

		if ( !this.IsAlive() )
			return;

		CreateFlashlight();
		Event.Run( TTTEvent.Player.Spawned, this );
	}

	public override void Simulate( Client client )
	{
		var controller = GetActiveController();
		controller?.Simulate( client, this, GetActiveAnimator() );

		if ( Input.ActiveChild is Carriable carriable )
			Inventory.SetActive( carriable );

		// SimulateCarriableSwitch();
		SimulateActiveChild( client, ActiveChild );

		if ( this.IsAlive() )
		{
			SimulateFlashlight();
			SimulatePerks();
		}

		if ( IsClient )
		{
			ActivateRoleButton();
		}
		else
		{
			CheckAFK();

			if ( !this.IsAlive() )
			{
				ChangeSpectateCamera();
				return;
			}

			PlayerUse();
			CheckPlayerDropCarriable();
			CheckLastSeenPlayer();
		}
	}

	public override void FrameSimulate( Client client )
	{
		var controller = GetActiveController();
		controller?.FrameSimulate( client, this, GetActiveAnimator() );

		if ( WaterLevel > 0.9f )
		{
			Audio.SetEffect( "underwater", 1, velocity: 5.0f );
		}
		else
		{
			Audio.SetEffect( "underwater", 0, velocity: 1.0f );
		}

		DisplayEntityHints();
		ActiveChild?.FrameSimulate( client );
	}

	/// <summary>
	/// Called after the camera setup logic has run. Allow the player to
	/// do stuff to the camera, or using the camera. Such as positioning entities
	/// relative to it, like viewmodels etc.
	/// </summary>
	public override void PostCameraSetup( ref CameraSetup setup )
	{
		ActiveChild?.PostCameraSetup( ref setup );
	}

	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput( InputBuilder input )
	{
		if ( input.StopProcessing )
			return;

		ActiveChild?.BuildInput( input );

		GetActiveController()?.BuildInput( input );

		if ( input.StopProcessing )
			return;

		GetActiveAnimator()?.BuildInput( input );
	}

	public void RenderHud( Vector2 screenSize )
	{
		if ( !this.IsAlive() )
			return;

		ActiveChild?.RenderHud( screenSize );
		UI.Crosshair.Instance?.RenderCrosshair( screenSize * 0.5, ActiveChild );
	}

	#region Animator
	[Net, Predicted]
	public PawnAnimator Animator { get; set; }

	public PawnAnimator GetActiveAnimator() => Animator;

	TimeSince _timeSinceLastFootstep;

	/// <summary>
	/// A foostep has arrived!
	/// </summary>
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( !this.IsAlive() )
			return;

		if ( !IsClient )
			return;

		if ( _timeSinceLastFootstep < 0.2f )
			return;

		volume *= FootstepVolume();

		_timeSinceLastFootstep = 0;

		var tr = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !tr.Hit )
			return;

		tr.Surface.DoFootstep( this, tr, foot, volume );
	}

	public float FootstepVolume()
	{
		return Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 0.2f;
	}
	#endregion

	#region Controller
	[Net, Predicted]
	public PawnController Controller { get; set; }

	[Net, Predicted]
	public PawnController DevController { get; set; }

	public PawnController GetActiveController()
	{
		return DevController ?? Controller;
	}
	#endregion

	public void CreateHull()
	{
		CollisionGroup = CollisionGroup.Player;
		AddCollisionLayer( CollisionLayer.Player );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		MoveType = MoveType.MOVETYPE_WALK;
		EnableHitboxes = true;
	}

	public override void StartTouch( Entity other )
	{
		if ( other is PickupTrigger )
		{
			Touch( other.Parent );

			return;
		}

		if ( !IsServer )
			return;

		switch ( other )
		{
			case Ammo ammo:
			{
				ammo.StartTouch( this );
				break;
			}
			case Carriable carriable:
			{
				Inventory.Pickup( carriable );
				break;
			}
		}
	}

	public void DeleteItems()
	{
		Components.RemoveAll();
		ClearAmmo();
		Inventory.DeleteContents();
		RemoveAllClothing();
	}

	[Net, Predicted]
	public Carriable ActiveChild { get; set; }

	public Carriable LastActiveChild { get; set; }

	public void SimulateActiveChild( Client client, Carriable child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( !LastActiveChild.IsAuthority )
			return;

		if ( LastActiveChild.TimeSinceDeployed > LastActiveChild.Info.DeployTime )
			LastActiveChild.Simulate( client );
	}

	public void OnActiveChildChanged( Carriable previous, Carriable next )
	{
		previous?.ActiveEnd( this, previous.Owner != this );
		next?.ActiveStart( this );
	}

	private void CheckPlayerDropCarriable()
	{
		if ( Input.Pressed( InputButton.Drop ) && !Input.Down( InputButton.Run ) )
		{
			var droppedEntity = Inventory.DropActive();

			if ( droppedEntity is not null )
			{
				if ( droppedEntity.PhysicsGroup is not null )
				{
					droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * DropVelocity;
				}
			}
		}
	}

	private void SimulatePerks()
	{
		foreach ( var perk in Perks )
		{
			perk.Simulate( Client );
		}
	}

	public override void OnChildAdded( Entity child )
	{
		Inventory?.OnChildAdded( child );
	}

	public override void OnChildRemoved( Entity child )
	{
		Inventory?.OnChildRemoved( child );
	}

	protected override void OnComponentAdded( EntityComponent component )
	{
		Perks?.OnComponentAdded( component );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		Perks?.OnComponentRemoved( component );
	}

	protected override void OnDestroy()
	{
		RemoveCorpse();
		DeleteFlashlight();

		base.OnDestroy();
	}

	[ClientRpc]
	public static void ClientRespawn( Player player )
	{
		if ( !player.IsValid() )
			return;

		player.ClientRespawn();
	}
}
