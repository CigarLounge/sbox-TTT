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

		base.Spawn();

		SetRole( new NoneRole() );
		SetModel( "models/citizen/citizen.vmdl" );
		Animator = new StandardPlayerAnimator();
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

		Credits = 0;
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;
		DeleteItems();
		SetRole( new NoneRole() );
		ClientRespawn();

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
			Client.SetValue( RawStrings.Spectator, false );
			Game.Current.Round.OnPlayerSpawned( this );
			ResetInterpolation();
			Game.Current?.MoveToSpawnpoint( this );
		}
		else
		{
			MakeSpectator( false );
		}
	}

	[ClientRpc]
	private void ClientRespawn()
	{
		IsConfirmedDead = false;
		IsMissingInAction = false;
		IsRoleKnown = false;

		if ( !IsLocalPawn )
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
		FlashlightEnabled = false;

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
			foreach ( var player in Utils.GetPlayers() )
			{
				Log.Info( player.Role.Info.Title );
			}

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

		SimulateFlashlight();
		SimulateCarriableSwitch();
		SimulatePerks();
		SimulateActiveChild( client, ActiveChild );

		var controller = GetActiveController();
		controller?.Simulate( client, this, GetActiveAnimator() );
	}

	public override void FrameSimulate( Client client )
	{
		base.FrameSimulate( client );

		CurrentPlayer.FrameSimulateFlashlight();
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
		DeactivateFlashlight();

		base.OnDestroy();
	}
}
