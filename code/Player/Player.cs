using Sandbox;

namespace TTT;

public partial class Player : Sandbox.Player
{
	[Net, Local, Predicted]
	public Entity LastActiveChild { get; set; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	public new Inventory Inventory
	{
		get => base.Inventory as Inventory;
		private init => base.Inventory = value;
	}

	public Perks Perks { get; private init; }

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
		SetRole( new NoneRole() );

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

		SetRole( new NoneRole() );
	}

	public override void Respawn()
	{
		Host.AssertServer();

		LifeState = LifeState.Respawnable;
		Client.SetValue( Constants.Game.Spectator, IsForcedSpectator );

		CleanRound = true;
		Confirmer = null;
		Corpse = null;
		LastSeenPlayerName = string.Empty;
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;

		DeleteFlashlight();
		DeleteItems();
		SetRole( new NoneRole() );

		Velocity = Vector3.Zero;
		WaterLevel = 0;
		Credits = 0;

		if ( !IsSpectator )
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

			Game.Current.Round.OnPlayerSpawned( this );
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

		Confirmer = null;
		Corpse = null;
		LastSeenPlayerName = string.Empty;
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;

		DeleteFlashlight();

		if ( this.IsAlive() )
			CreateFlashlight();

		if ( !IsLocalPawn )
			SetRole( new NoneRole() );
		else
			ClearButtons();
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

		Game.Current.Round.OnPlayerKilled( this );
		Role?.OnKilled( this );
		Event.Run( TTTEvent.Player.Killed, this );

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

		if ( Input.Pressed( InputButton.Menu ) )
		{
			if ( ActiveChild.IsValid() && LastActiveChild.IsValid() )
				(ActiveChild, LastActiveChild) = (LastActiveChild, ActiveChild);
		}

		if ( this.IsAlive() )
		{
			SimulateFlashlight();
			SimulateCarriableSwitch();
			SimulatePerks();
		}

		SimulateActiveChild( client, ActiveChild );
		var controller = GetActiveController();
		controller?.Simulate( client, this, GetActiveAnimator() );
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
		RemoveClothing();
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
		if ( Input.ActiveChild is not null )
		{
			LastActiveChild = ActiveChild;
			ActiveChild = Input.ActiveChild;
		}
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

		if ( Host.IsClient && component is Perk perk )
			Perks.Add( perk );
	}

	protected override void OnComponentRemoved( EntityComponent component )
	{
		base.OnComponentAdded( component );

		if ( component is Perk perk )
			Perks.Remove( perk );
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
