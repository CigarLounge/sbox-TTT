using Sandbox;

namespace TTT;

public partial class Player : Sandbox.Player
{
	[BindComponent]
	public Perks Perks { get; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	[Net]
	public bool IsForcedSpectator { get; set; } = false;

	public new Inventory Inventory
	{
		get => base.Inventory as Inventory;
		private init => base.Inventory = value;
	}

	private static int CarriableDropVelocity { get; set; } = 300;

	public Player()
	{
		Inventory = new Inventory( this );
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/citizen/citizen.vmdl" );
		Components.GetOrCreate<Perks>();
	}

	public override void Respawn()
	{
		base.Respawn();

		Animator = new StandardPlayerAnimator();
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = true;
		Credits = 0;
		SetRole( new NoneRole() );
		IsMissingInAction = false;
		DeleteItems();
		ClientRespawn();

		if ( !IsForcedSpectator )
		{
			Controller = new WalkController();
			Camera = new FirstPersonCamera();
			EnableAllCollisions = true;
		}
		else
		{
			MakeSpectator( false );
		}

		Game.Current.Round.OnPlayerSpawn( this );
	}

	[ClientRpc]
	private void ClientRespawn()
	{
		IsMissingInAction = false;
		IsConfirmed = false;

		SetRole( new NoneRole() );
	}

	// Let's clean this up at some point, it's poorly written.
	public override void OnKilled()
	{
		base.OnKilled();

		BecomePlayerCorpseOnServer();

		Inventory.DropAll();
		DeleteItems();
		IsMissingInAction = true;

		RPCs.ClientOnPlayerDied( this );
		Role?.OnKilled( LastAttacker as Player );

		if ( Game.Current.Round is InProgressRound )
			SyncMIA();
		else if ( Game.Current.Round is PostRound )
			Corpse.Confirm();
	}

	public override void Simulate( Client client )
	{
		TickPerkSimulate();
		TickPlayerDropCarriable();

		if ( IsClient )
		{
			TickPlayerVoiceChat();
			TickEntityHints();
			TickLogicButtonActivate();
		}
		else
		{
			TickPlayerUse();
			TickAFKSystem();

			if ( !this.IsAlive() )
			{
				TickPlayerChangeSpectateCamera();
				return;
			}
		}

		// Input requested a carriable entity switch
		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

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

	private void TickPlayerDropCarriable()
	{
		if ( Input.Pressed( InputButton.Drop ) && !Input.Down( InputButton.Run ) )
		{
			Entity droppedEntity = Inventory.DropActive();

			if ( droppedEntity != null )
			{
				if ( droppedEntity.PhysicsGroup != null )
				{
					droppedEntity.PhysicsGroup.Velocity = Velocity + (EyeRotation.Forward + EyeRotation.Up) * CarriableDropVelocity;
				}
			}
		}
	}

	private void TickPerkSimulate()
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
