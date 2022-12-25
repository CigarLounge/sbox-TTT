using Sandbox;

namespace TTT;

[Title( "Player" ), Icon( "emoji_people" )]
public partial class Player : AnimatedEntity
{
	public Inventory Inventory { get; private init; }
	public Perks Perks { get; private init; }

	[ClientInput]
	public Vector3 InputDirection { get; set; }

	[ClientInput]
	public Entity ActiveChildInput { get; set; }

	[ClientInput]
	public Angles ViewAngles { get; set; }
	public Angles OriginalViewAngles { get; private set; }

	public Vector3 EyePosition
	{
		get => Transform.PointToWorld( EyeLocalPosition );
		set => EyeLocalPosition = Transform.PointToLocal( value );
	}

	/// <summary>
	/// Position a player should be looking from in local to the entity coordinates.
	/// </summary>
	[Net, Predicted]
	public Vector3 EyeLocalPosition { get; set; }

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity.
	/// </summary>
	public Rotation EyeRotation
	{
		get => Transform.RotationToWorld( EyeLocalRotation );
		set => EyeLocalRotation = Transform.RotationToLocal( value );
	}

	/// <summary>
	/// Rotation of the entity's "eyes", i.e. rotation for the camera when this entity is used as the view entity. In local to the entity coordinates.
	/// </summary>
	[Net, Predicted]
	public Rotation EyeLocalRotation { get; set; }

	/// <summary>
	/// Override the aim ray to use the player's eye position and rotation.
	/// </summary>
	public override Ray AimRay => new( EyePosition, EyeRotation.Forward );

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

	public Player( IClient client ) : this()
	{
		client.Pawn = this;
		SteamId = client.SteamId;
		SteamName = client.Name;
		ActiveKarma = BaseKarma;

		ClothingContainer.LoadFromClient( client );
		_avatarClothes = new( ClothingContainer.Clothing );
	}

	public Player()
	{
		Inventory = new( this );
		Perks = new( this );
	}

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "ignorereset" );
		Tags.Add( "player" );
		Tags.Add( "solid" );

		if ( GameManager.HumanAvatar )
		{
			SetModel( "models/humans/male.vmdl" );

			var useLightSkinTone = Game.Random.Int( 0, 1 ) == 1;
			if ( useLightSkinTone )
				SetMaterialGroup( "skin1" );

			var head = new AnimatedEntity();
			head.Model = Model.Load( useLightSkinTone ? "models/humans/heads/adam/adam.vmdl" : "models/humans/heads/frank/frank.vmdl" );
			head.EnableHideInFirstPerson = true;
			head.EnableShadowInFirstPerson = true;
			head.SetParent( this, true );
		}
		else
		{
			SetModel( "models/citizen/citizen.vmdl" );
			ClothingContainer.Clothing = GameManager.AvatarClothing ? _avatarClothes : _currentPreset;
			ClothingContainer.DressEntity( this );
		}


		Role = new NoneRole();

		Health = 0;
		LifeState = LifeState.Respawnable;
		Transmit = TransmitType.Always;

		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableLagCompensation = true;
		EnableShadowInFirstPerson = true;
		EnableTouch = false;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		Role = new NoneRole();
	}

	public void Respawn()
	{
		Game.AssertServer();

		LifeState = LifeState.Respawnable;

		DeleteFlashlight();
		DeleteItems();
		ResetConfirmationData();
		ResetDamageData();
		Client.SetValue( Strings.Spectator, IsForcedSpectator );
		Role = new NoneRole();

		Velocity = Vector3.Zero;
		Credits = 0;

		if ( !IsForcedSpectator )
		{
			Health = MaxHealth;
			Status = PlayerStatus.Alive;
			Client.Voice.WantsStereo = GameManager.ProximityChat;
			UpdateStatus( To.Everyone );
			LifeState = LifeState.Alive;

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableTouch = true;

			Controller = new WalkController();

			CreateHull();
			CreateFlashlight();
			//DressPlayer();
			ResetInterpolation();

			Event.Run( GameEvent.Player.Spawned, this );
			GameManager.Current.State.OnPlayerSpawned( this );
		}
		else
		{
			Status = PlayerStatus.Spectator;
			UpdateStatus( To.Everyone );
			MakeSpectator();
		}

		ClientRespawn( this );
	}

	private void ClientRespawn()
	{
		Game.AssertClient();

		DeleteFlashlight();
		ResetConfirmationData();
		ResetDamageData();

		if ( !IsLocalPawn )
			Role = new NoneRole();
		else
		{
			CurrentChannel = Channel.All;
			MuteFilter = MuteFilter.None;
			ClearButtons();
		}

		if ( IsSpectator )
			return;

		if ( IsLocalPawn )
			CurrentCamera = new FirstPersonCamera( this );

		CreateFlashlight();

		Event.Run( GameEvent.Player.Spawned, this );
	}

	public override void Simulate( IClient client )
	{
		SimulateAnimation( Controller );

		if ( Input.Pressed( InputButton.Menu ) )
		{
			if ( ActiveCarriable.IsValid() && _lastKnownCarriable.IsValid() )
				(ActiveCarriable, _lastKnownCarriable) = (_lastKnownCarriable, ActiveCarriable);
		}

		if ( ActiveChildInput is Carriable carriable )
			Inventory.SetActive( carriable );

		SimulateActiveCarriable();

		if ( this.IsAlive() )
		{
			Controller?.SetActivePlayer( this );
			Controller?.Simulate();
			SimulateFlashlight();
			SimulatePerks();
		}

		if ( Game.IsClient )
		{
			ActivateRoleButton();
		}
		else
		{
			CheckAFK();

			if ( !this.IsAlive() )
			{
				if ( Prop.IsValid() )
					SimulatePossession();

				return;
			}

			PlayerUse();
			CheckLastSeenPlayer();
			CheckPlayerDropCarriable();
		}
	}

	public override void FrameSimulate( IClient client )
	{
		Controller?.SetActivePlayer( this );
		Controller?.FrameSimulate();
		CurrentCamera?.FrameSimulate( this );
	}

	/// <summary>
	/// Called from the gamemode, clientside only.
	/// </summary>
	public override void BuildInput()
	{
		OriginalViewAngles = ViewAngles;
		InputDirection = Input.AnalogMove;

		if ( Input.StopProcessing )
			return;

		var look = Input.AnalogLook;

		if ( ViewAngles.pitch > 90f || ViewAngles.pitch < -90f )
			look = look.WithYaw( look.yaw * -1f );

		var viewAngles = ViewAngles;
		viewAngles += look;
		viewAngles.pitch = viewAngles.pitch.Clamp( -89f, 89f );
		viewAngles.roll = 0f;
		ViewAngles = viewAngles.Normal;

		DisplayEntityHints();
		ActiveCarriable?.BuildInput();
		CurrentCamera?.BuildInput( this );
	}

	TimeSince _timeSinceLastFootstep;

	/// <summary>
	/// A foostep has arrived!
	/// </summary>
	public override void OnAnimEventFootstep( Vector3 pos, int foot, float volume )
	{
		if ( !this.IsAlive() )
			return;

		if ( !Game.IsClient )
			return;

		if ( _timeSinceLastFootstep < 0.2f )
			return;

		volume *= FootstepVolume();

		_timeSinceLastFootstep = 0;

		var trace = Trace.Ray( pos, pos + Vector3.Down * 20 )
			.Radius( 1 )
			.Ignore( this )
			.Run();

		if ( !trace.Hit )
			return;

		trace.Surface.DoFootstep( this, trace, foot, volume );
	}

	public float FootstepVolume()
	{
		return Velocity.WithZ( 0 ).Length.LerpInverse( 0.0f, 200.0f ) * 5.0f;
	}

	#region Controller
	[Net, Predicted]
	public WalkController Controller { get; set; }

	private void SimulateAnimation( WalkController controller )
	{
		if ( controller == null )
			return;

		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		// If we're a bot, spin us around 180 degrees.
		if ( Client.IsBot )
			rotation = ViewAngles.WithYaw( ViewAngles.yaw + 180f ).ToRotation();
		else
			rotation = ViewAngles.ToRotation();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp( Rotation, idealRotation, controller.WishVelocity.Length * Time.Delta * turnSpeed );
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		var animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( controller.WishVelocity );
		animHelper.WithVelocity( Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp( animHelper.DuckLevel, controller.HasTag( "ducked" ) ? 1 : 0, Time.Delta * 10.0f );
		animHelper.VoiceLevel = (Game.IsClient && Client.IsValid()) ? Client.Voice.LastHeard < 0.5f ? Client.Voice.CurrentLevel : 0.0f : 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSitting = controller.HasTag( "sitting" );
		animHelper.IsNoclipping = controller.HasTag( "noclip" );
		animHelper.IsClimbing = controller.HasTag( "climbing" );
		animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( controller.HasEvent( "jump" ) )
			animHelper.TriggerJump();

		if ( ActiveCarriable != _lastActiveCarriable )
			animHelper.TriggerDeploy();

		if ( ActiveCarriable is not null )
			ActiveCarriable.SimulateAnimator( animHelper );
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}
	}
	#endregion

	public void CreateHull()
	{
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );
		EnableHitboxes = true;
	}

	public override void StartTouch( Entity other )
	{
		if ( !Game.IsServer )
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
		ClearAmmo();
		Inventory.DeleteContents();
		Perks.DeleteContents();
		ClothingContainer.ClearEntities();
	}

	#region ActiveCarriable
	[Net, Predicted]
	public Carriable ActiveCarriable { get; set; }

	public Carriable _lastActiveCarriable;
	public Carriable _lastKnownCarriable;

	public void SimulateActiveCarriable()
	{
		if ( _lastActiveCarriable != ActiveCarriable )
		{
			OnActiveCarriableChanged( _lastActiveCarriable, ActiveCarriable );
			_lastKnownCarriable = _lastActiveCarriable;
			_lastActiveCarriable = ActiveCarriable;
		}

		if ( !ActiveCarriable.IsValid() || !ActiveCarriable.IsAuthority )
			return;

		if ( ActiveCarriable.TimeSinceDeployed > ActiveCarriable.Info.DeployTime )
			ActiveCarriable.Simulate( Client );
	}

	public void OnActiveCarriableChanged( Carriable previous, Carriable next )
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
				if ( droppedEntity.PhysicsGroup is not null )
					droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * DropVelocity;
		}
	}
	#endregion

	private void SimulatePerks()
	{
		foreach ( var perk in Perks )
			perk.Simulate( Client );
	}

	public override void OnChildAdded( Entity child )
	{
		if ( child is Carriable carriable )
			Inventory.OnChildAdded( carriable );
	}

	public override void OnChildRemoved( Entity child )
	{
		if ( child is Carriable carriable )
			Inventory.OnChildRemoved( carriable );
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
		if ( Game.IsServer )
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
