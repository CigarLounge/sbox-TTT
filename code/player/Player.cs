using Sandbox;

namespace TTT;

public partial class Player : Sandbox.Player
{
	[Net, Predicted]
	public Entity LastActiveChild { get; set; }

	[BindComponent]
	public Perks Perks { get; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	public new Inventory Inventory
	{
		get => base.Inventory as Inventory;
		private init => base.Inventory = value;
	}

	public static int DropVelocity { get; set; } = 300;

	public Player()
	{
		Inventory = new Inventory( this );
	}

	public override void Spawn()
	{
		Components.GetOrCreate<Perks>();

		SetModel( "models/citizen/citizen.vmdl" );
		Animator = new StandardPlayerAnimator();
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		CameraMode = new FreeSpectateCamera();

		base.Spawn();
	}

	public override void Respawn()
	{
		Host.AssertServer();

		EnableDrawing = true;
		EnableAllCollisions = true;
		Credits = 0;
		SetRole( new NoneRole() );
		IsMissingInAction = false;
		DeleteItems();
		ClientRespawn();

		if ( !IsForcedSpectator )
		{
			Controller = new WalkController();
			CameraMode = new FirstPersonCamera();
			EnableAllCollisions = true;
			Client.SetValue( RawStrings.Spectator, false );
			AttachClothing( "models/citizen_clothes/hat/balaclava/models/balaclava.vmdl" );
			AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
			AttachClothing( "models/citizen_clothes/jacket/longsleeve/models/longsleeve.vmdl" );
			AttachClothing( "models/citizen_clothes/trousers/smarttrousers/smarttrousers.vmdl" );
			AttachClothing( "models/citizen_clothes/shoes/shoes.smartbrown.vmdl" );
			AttachClothing( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.vmdl" );
		}
		else
		{
			MakeSpectator( false );
		}

		Game.Current.Round.OnPlayerSpawned( this );

		LifeState = LifeState.Alive;
		Health = MaxHealth;
		Velocity = Vector3.Zero;
		WaterLevel = 0;

		CreateHull();

		Game.Current?.MoveToSpawnpoint( this );
		ResetInterpolation();
	}

	[ClientRpc]
	private void ClientRespawn()
	{
		IsMissingInAction = false;
		IsConfirmed = false;
		SetRole( new NoneRole() );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomePlayerCorpseOnServer();
		RemoveAllDecals();

		Inventory.DropAll();
		DeleteItems();
		IsMissingInAction = true;

		Game.Current.Round.OnPlayerKilled( this );
		Role?.OnKilled( this );
		RPCs.ClientOnPlayerDied( this );

		if ( Game.Current.Round is InProgressRound )
			SyncMIA();
		else if ( Game.Current.Round is PostRound )
			Corpse.Confirm();
	}

	public override void Simulate( Client client )
	{
		if ( IsClient )
		{
			DisplayEntityHints();
			LogicButtonActivate();
		}
		else
		{
			PlayerUse();
			CheckAFK();
			CheckPlayerDropCarriable();

			if ( !this.IsAlive() )
			{
				ChangeSpectateCamera();
				return;
			}
		}

		if ( Input.Pressed( InputButton.Menu ) )
		{
			if ( ActiveChild.IsValid() && LastActiveChild.IsValid() )
				(ActiveChild, LastActiveChild) = (LastActiveChild, ActiveChild);
		}

		SimulateCarriableSwitch();
		SimulatePerks();
		SimulateActiveChild( client, ActiveChild );

		PawnController controller = GetActiveController();
		controller?.Simulate( client, this, GetActiveAnimator() );
	}

	public override void StartTouch( Entity other )
	{
		if ( other is PickupTrigger )
		{
			StartTouch( other.Parent );

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
			Entity droppedEntity = Inventory.DropActive();

			if ( droppedEntity != null )
			{
				if ( droppedEntity.PhysicsGroup != null )
				{
					droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * DropVelocity;
				}
			}
		}
	}

	private void SimulateCarriableSwitch()
	{
		if ( Input.ActiveChild != null )
		{
			LastActiveChild = ActiveChild;
			ActiveChild = Input.ActiveChild;
		}
	}

	private void SimulatePerks()
	{
		for ( int i = 0; i < Perks.Count; ++i )
		{
			Perks.Get( i ).Simulate( this );
		}
	}

	protected override void OnDestroy()
	{
		RemovePlayerCorpse();

		base.OnDestroy();
	}
}
