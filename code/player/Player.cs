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

		base.Spawn();
	}

	public override void Respawn()
	{
		base.Respawn();

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
		}
		else
		{
			MakeSpectator( false );
		}

		Game.Current.Round.OnPlayerSpawned( this );
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
		Role?.OnKilled( LastAttacker as Player );
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
