using Sandbox;

using TTT.Player.Camera;
using TTT.Roles;

namespace TTT.Player;

public partial class TTTPlayer : Sandbox.Player
{
	[BindComponent]
	public Perks Perks { get; }

	[Net, Local]
	public int Credits { get; set; } = 0;

	[Net]
	public bool IsForcedSpectator { get; set; } = false;

	public bool IsInitialSpawning { get; set; } = false;

	public new Inventory Inventory
	{
		get => base.Inventory as Inventory;
		private init => base.Inventory = value;
	}

	public new DefaultWalkController Controller
	{
		get => base.Controller as DefaultWalkController;
		private set => base.Controller = value;
	}

	private static int CarriableDropVelocity { get; set; } = 300;
	private DamageInfo _lastDamageInfo;

	public TTTPlayer()
	{
		Inventory = new Inventory( this );
		Role = new NoneRole();
	}

	public override void Spawn()
	{
		Components.GetOrCreate<Perks>();
		base.Spawn();
	}

	// Important: Server-side only
	public void InitialSpawn()
	{
		bool isPostRound = Gamemode.Game.Current.Round is Rounds.PostRound;

		IsInitialSpawning = true;
		IsForcedSpectator = isPostRound || Gamemode.Game.Current.Round is Rounds.InProgressRound;

		Respawn();

		// sync roles
		using ( Prediction.Off() )
		{
			foreach ( TTTPlayer player in Utils.GetPlayers() )
			{
				if ( isPostRound || player.IsConfirmed )
				{
					player.SendClientRole( To.Single( this ) );
				}
			}

			Client.SetValue( "forcedspectator", IsForcedSpectator );
		}

		IsInitialSpawning = false;
		IsForcedSpectator = false;
	}

	// Let's clean this up at some point, it's poorly written.
	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Animator = new StandardPlayerAnimator();

		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		EnableDrawing = true;

		Credits = 0;

		SetRole( new NoneRole() );

		IsMissingInAction = false;

		using ( Prediction.Off() )
		{
			RPCs.ClientOnPlayerSpawned( this );
			SendClientRole();
		}

		base.Respawn();

		if ( !IsForcedSpectator )
		{
			Controller = new DefaultWalkController();
			Camera = new FirstPersonCamera();
			EnableAllCollisions = true;
		}
		else
		{
			MakeSpectator( false );
		}

		DeleteItems();
		Gamemode.Game.Current.Round.OnPlayerSpawn( this );

		switch ( Gamemode.Game.Current.Round )
		{
			// hacky
			// TODO use a spectator flag, otherwise, no player can respawn during round with an item etc.
			// TODO spawn player as spectator instantly
			case Rounds.PreRound:
				IsConfirmed = false;
				CorpseConfirmer = null;

				Client.SetValue( "forcedspectator", false );

				break;
		}
	}

	// Let's clean this up at some point, it's poorly written.
	public override void OnKilled()
	{
		base.OnKilled();

		BecomePlayerCorpseOnServer( _lastDamageInfo.Force, GetHitboxBone( _lastDamageInfo.HitboxIndex ) );

		Inventory.DropAll();
		DeleteItems();

		IsMissingInAction = true;

		using ( Prediction.Off() )
		{
			RPCs.ClientOnPlayerDied( this );
			Role?.OnKilled( _lastDamageInfo.Attacker as TTTPlayer );

			if ( Gamemode.Game.Current.Round is Rounds.InProgressRound )
			{
				SyncMIA();
			}
			else if ( Gamemode.Game.Current.Round is Rounds.PostRound && PlayerCorpse != null && !PlayerCorpse.IsIdentified )
			{
				PlayerCorpse.IsIdentified = true;

				RPCs.ClientConfirmPlayer( null, PlayerCorpse, this, PlayerCorpse.DeadPlayerClientData.Name, PlayerCorpse.DeadPlayerClientData.PlayerId, Role.ClassInfo.Name, PlayerCorpse.GetConfirmationData(), PlayerCorpse.KillerWeapon.LibraryName, PlayerCorpse.Perks );
			}
		}
	}

	public override void Simulate( Client client )
	{
		if ( IsClient )
		{
			TickPlayerVoiceChat();
		}
		else
		{
			TickAFKSystem();
		}

		TickEntityHints();

		if ( LifeState != LifeState.Alive )
		{
			TickPlayerChangeSpectateCamera();

			return;
		}

		// Input requested a carriable entity switch
		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		SimulateActiveChild( client, ActiveChild );

		TickPerkSimulate();
		TickPlayerUse();
		TickPlayerDropCarriable();
		// TickPlayerShop();
		TickLogicButtonActivate();

		PawnController controller = GetActiveController();
		controller?.Simulate( client, this, GetActiveAnimator() );
	}

	public override void StartTouch( Entity other )
	{
		if ( IsClient )
			return;

		if ( other is PickupTrigger )
			StartTouch( other.Parent );
	}

	public void DeleteItems()
	{
		Perks.Clear();
		ClearAmmo();
		Inventory.DeleteContents();
		RemoveClothing();
	}

	private void TickPlayerDropCarriable()
	{
		if ( Input.Pressed( InputButton.Drop ) && !Input.Down( InputButton.Run ) && ActiveChild != null && Inventory != null )
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

	private void TickPlayerChangeSpectateCamera()
	{
		if ( !Input.Pressed( InputButton.Jump ) || !IsServer )
		{
			return;
		}

		using ( Prediction.Off() )
		{
			Camera = Camera switch
			{
				RagdollSpectateCamera => new FreeSpectateCamera(),
				FreeSpectateCamera => new ThirdPersonSpectateCamera(),
				ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
				FirstPersonSpectatorCamera => new FreeSpectateCamera(),
				_ => Camera
			};
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
