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

		Animator = new StandardPlayerAnimator();
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

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeCorpse();
		RemoveAllDecals();

		EnableAllCollisions = false;
		EnableDrawing = false;

		Inventory.DropAll();
		DeleteFlashlight();
		DeleteItems();

		Event.Run( TTTEvent.Player.Killed, this );
		Game.Current.State.OnPlayerKilled( this );

		ClientOnKilled( this );
	}

	private void ClientOnKilled()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
			ClearButtons();

		DeleteFlashlight();
		Event.Run( TTTEvent.Player.Killed, this );
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

		SimulateActiveChild( client, ActiveChild );

		if ( this.IsAlive() )
		{
			SimulateFlashlight();
			SimulateCarriableSwitch();
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
		base.FrameSimulate( client );

		DisplayEntityHints();
		ActiveChild?.FrameSimulate( client );
	}

	public override void StartTouch( Entity other )
	{
		if ( other is PickupTrigger )
		{
			Touch( other.Parent );

			return;
		}

		if ( IsServer )
			Inventory.Pickup( other );
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

	[ClientRpc]
	public static void ClientOnKilled( Player player )
	{
		if ( !player.IsValid() )
			return;

		player.ClientOnKilled();
	}
}
