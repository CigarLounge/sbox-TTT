using Sandbox;
using System.Linq;

namespace TTT;

public enum SlotType
{
	Primary,
	Secondary,
	Melee,
	OffensiveEquipment,
	UtilityEquipment,
	Grenade,
}

public enum HoldType
{
	None,
	Pistol,
	Rifle,
	Shotgun,
	Carry,
	Fists
}

[Title( "Carriable" ), Icon( "luggage" )]
public abstract partial class Carriable : AnimatedEntity, IEntityHint, IUse
{
	[Net, Local, Predicted]
	public TimeSince TimeSinceDeployed { get; private set; }

	public TimeSince TimeSinceDropped { get; private set; }

	public new Player Owner
	{
		get => (Player)base.Owner;
		set => base.Owner = value;
	}

	public BaseViewModel HandsModelEntity { get; private set; }
	public CarriableInfo Info { get; private set; }
	public Player PreviousOwner { get; private set; }
	public BaseViewModel ViewModelEntity { get; protected set; }

	/// <summary>
	/// Utility - return the entity we should be spawning particles from etc
	/// </summary>
	public virtual ModelEntity EffectEntity => (ViewModelEntity.IsValid() && IsFirstPersonMode) ? ViewModelEntity : this;

	/// <summary>
	/// The text that will show up in the inventory slot.
	/// </summary>
	public virtual string SlotText => string.Empty;
	public bool IsActiveChild => Owner?.ActiveChild == this;

	public override void Spawn()
	{
		base.Spawn();

		Tags.Add( "trigger" );
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		if ( string.IsNullOrWhiteSpace( ClassName ) )
		{
			Log.Error( this + " doesn't have a class name!" );
			return;
		}

		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
		Model = Info.WorldModel;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrWhiteSpace( ClassName ) )
			Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}

	public override void StartTouch( Entity other )
	{
		if ( !IsServer )
			return;

		if ( other is Player player && player.IsAlive() && (player != PreviousOwner || TimeSinceDropped >= 1f) )
			player.Inventory.Add( this );
	}

	public virtual void ActiveStart( Player player )
	{
		EnableDrawing = true;

		var animator = player.Animator;

		if ( animator is not null )
			SimulateAnimator( animator );

		if ( IsLocalPawn )
		{
			CreateViewModel();
			CreateHudElements();

			ViewModelEntity?.SetAnimParameter( "deploy", true );
		}

		TimeSinceDeployed = 0;

		if ( Host.IsClient )
			return;

		if ( !Components.GetAll<DNA>().Any( ( dna ) => dna.TargetPlayer == Owner ) )
			Components.Add( new DNA( Owner ) );
	}

	public virtual void ActiveEnd( Player player, bool dropped )
	{
		if ( !dropped )
		{
			EnableDrawing = false;
		}

		if ( IsClient )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	public override void Simulate( Client client ) { }

	public override void FrameSimulate( Client client ) { }

	public virtual void RenderHud( in Vector2 screenSize ) { }

	public override void BuildInput( InputBuilder input ) { }

	/// <summary>
	/// Create the viewmodel. You can override this in your base classes if you want
	/// to create a certain viewmodel entity.
	/// </summary>
	public virtual void CreateViewModel()
	{
		Host.AssertClient();

		if ( Info.ViewModel is not null )
		{
			ViewModelEntity = new ViewModel
			{
				EnableViewmodelRendering = true,
				Model = Info.ViewModel,
				Owner = Owner,
				Position = Position
			};
		}

		if ( Info.HandsModel is not null )
		{
			HandsModelEntity = new BaseViewModel
			{
				EnableViewmodelRendering = true,
				Model = Info.HandsModel,
				Owner = Owner,
				Position = Position
			};

			HandsModelEntity.SetParent( ViewModelEntity, true );
		}
	}

	/// <summary>
	/// We're done with the viewmodel - delete it
	/// </summary>
	public virtual void DestroyViewModel()
	{
		ViewModelEntity?.Delete();
		ViewModelEntity = null;
		HandsModelEntity?.Delete();
		HandsModelEntity = null;
	}

	public virtual void CreateHudElements() { }

	public virtual void DestroyHudElements() { }

	public virtual bool CanCarry( Player carrier )
	{
		if ( Owner is not null )
			return false;

		if ( carrier == PreviousOwner && TimeSinceDropped < 1f )
			return false;

		return true;
	}

	public virtual void OnCarryStart( Player carrier )
	{
		// Bandaid fix for: https://github.com/Facepunch/sbox-issues/issues/1702
		if ( IsClient )
			Info ??= GameResource.GetInfo<CarriableInfo>( GetType() );

		if ( !IsServer )
			return;

		Owner = carrier;
		EnableAllCollisions = false;
		EnableDrawing = false;
		Sound.FromEntity( Strings.WeaponPickupSound, Owner );
	}

	public virtual void OnCarryDrop( Player dropper )
	{
		PreviousOwner = dropper;

		if ( !IsServer )
			return;

		Owner = null;
		EnableDrawing = true;
		EnableAllCollisions = true;
		TimeSinceDropped = 0;
	}

	public virtual void SimulateAnimator( PawnAnimator animator )
	{
		animator.SetAnimParameter( "holdtype", (int)Info.HoldType );
		animator.SetAnimParameter( "aim_body_weight", 1.0f );
		animator.SetAnimParameter( "holdtype_handedness", 0 );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( IsFirstPersonMode )
		{
			DestroyViewModel();
			DestroyHudElements();
		}
	}

	bool IEntityHint.CanHint( Player player ) => Owner is null;

	bool IUse.OnUse( Entity user )
	{
		var player = (Player)user;
		player.Inventory.OnUse( this );

		return false;
	}

	bool IUse.IsUsable( Entity user ) => Owner is null && user is Player;

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotload()
	{
		Info = GameResource.GetInfo<CarriableInfo>( ClassName );
	}
#endif
}
