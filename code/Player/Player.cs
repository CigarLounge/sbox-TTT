using Sandbox;

namespace TTT;

public partial class Player : Sandbox.Player
{
	[Net, Local, Predicted]
	public Entity LastActiveChild { get; set; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	private Inventory _inventory;
	public new Inventory Inventory
	{
		get => _inventory;
		private init
		{
			_inventory = value;
			base.Inventory = value;
		}
	}

	public Perks Perks { get; private init; }

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

		SetModel( "models/citizen/citizen.vmdl" );
		Role = new NoneRole();

		Health = 0;
		LifeState = LifeState.Respawnable;
		Transmit = TransmitType.Always;

		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		Animator = new PlayerAnimator();
		CameraMode = new FreeSpectateCamera();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Role = new NoneRole();
	}

	public override void Respawn()
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
			CameraMode = new FirstPersonCamera();

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

		if ( Input.Pressed( InputButton.Menu ) )
		{
			if ( ActiveChild.IsValid() && LastActiveChild.IsValid() )
				(ActiveChild, LastActiveChild) = (LastActiveChild, ActiveChild);
		}

		SimulateCarriableSwitch();
		SimulateActiveChild( client, ActiveChild );

		if ( this.IsAlive() )
		{
			SimulateFlashlight();
			SimulatePerks();
			CheckLastSeenPlayer();
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

	public override void OnActiveChildChanged( Entity previous, Entity next )
	{
		if ( previous is Carriable previousBc )
			previousBc?.ActiveEnd( this, previousBc.Owner != this );

		if ( next is Carriable nextBc )
			nextBc?.ActiveStart( this );
	}

	public void RenderHud( Vector2 screenSize )
	{
		if ( !this.IsAlive() )
			return;

		if ( ActiveChild is Carriable carriable )
			carriable.RenderHud( screenSize );

		UI.Crosshair.Instance?.RenderCrosshair( screenSize * 0.5, ActiveChild );
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
		Inventory?.DeleteContents();
		RemoveAllClothing();
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

	private void SimulateCarriableSwitch()
	{
		if ( Input.ActiveChild is null )
			return;

		LastActiveChild = ActiveChild;
		ActiveChild = Input.ActiveChild;
	}

	private void SimulatePerks()
	{
		foreach ( var perk in Perks )
		{
			perk.Simulate( Client );
		}
	}

	protected override void OnComponentAdded( EntityComponent component )
	{
		base.OnComponentAdded( component );

		Perks?.OnComponentAdded( component );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		base.OnComponentAdded( component );

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
