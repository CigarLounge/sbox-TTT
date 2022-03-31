using Sandbox;

namespace TTT;

public partial class Player : Sandbox.Player
{
	[Net, Predicted]
	public Entity LastActiveChild { get; set; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	public new Inventory Inventory
	{
		get => base.Inventory as Inventory;
		private init => base.Inventory = value;
	}

	public Perks Perks { get; init; }

	public const float DropVelocity = 300;

	public Player()
	{
		Inventory = new( this );
		Perks = new( this );
	}

	public override void Spawn()
	{
		Transmit = TransmitType.Always;

		base.Spawn();

		SetRole( new NoneRole() );
		SetModel( "models/citizen/citizen.vmdl" );
		Animator = new StandardPlayerAnimator();
		Health = 0;
		LifeState = LifeState.Respawnable;
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		CameraMode = new FreeSpectateCamera();
		ActivateFlashlight();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		ActivateFlashlight();
		SetRole( new NoneRole() );
	}

	public override void Respawn()
	{
		Host.AssertServer();

		LifeState = LifeState.Respawnable;
		Client.SetValue( RawStrings.Spectator, IsForcedSpectator );
		Credits = 0;
		Confirmer = null;
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;
		DeleteItems();
		SetRole( new NoneRole() );

		Velocity = Vector3.Zero;
		WaterLevel = 0;

		if ( !IsForcedSpectator )
		{
			LifeState = LifeState.Alive;
			Health = MaxHealth;
			EnableAllCollisions = true;
			EnableDrawing = true;
			Controller = new WalkController();
			CameraMode = new FirstPersonCamera();
			DressPlayer();
			CreateHull();
			Game.Current.Round.OnPlayerSpawned( this );
			ResetInterpolation();
			Game.Current.MoveToSpawnpoint( this );
		}
		else
		{
			MakeSpectator( false );
		}

		ClientRespawn( this );
	}

	[ClientRpc]
	public static void ClientRespawn( Player player )
	{
		if ( !player.IsValid() )
			return;

		player.Confirmer = null;
		player.IsConfirmedDead = false;
		player.IsMissingInAction = false;
		player.IsRoleKnown = false;

		if ( !player.IsLocalPawn )
			player.SetRole( new NoneRole() );
		else
			player.ClearButtons();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomePlayerCorpseOnServer();
		RemoveAllDecals();

		Inventory.DropAll();
		DeleteItems();
		FlashlightEnabled = false;

		Game.Current.Round.OnPlayerKilled( this );
		Role?.OnKilled( this );
		ClientOnKilled( this );
	}

	[ClientRpc]
	public static void ClientOnKilled( Player player )
	{
		if ( !player.IsValid() )
			return;

		Event.Run( TTTEvent.Player.Killed, player );
	}

	public override void Simulate( Client client )
	{
		if ( IsClient )
		{
			ActivateRoleButton();
		}
		else
		{
			if ( !this.IsAlive() )
			{
				ChangeSpectateCamera();
				return;
			}

			PlayerUse();
			CheckAFK();
			CheckPlayerDropCarriable();
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
		CurrentPlayer.FrameSimulateFlashlight();
	}

	public override void Touch( Entity other )
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
		Perks.Clear();
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
		for ( int i = 0; i < Perks.Count; ++i )
		{
			Perks.Get( i ).Simulate( Client );
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

		if ( Host.IsClient && component is Perk perk )
			Perks.Remove( perk );
	}

	protected override void OnDestroy()
	{
		RemovePlayerCorpse();
		DeactivateFlashlight();

		base.OnDestroy();
	}
}
