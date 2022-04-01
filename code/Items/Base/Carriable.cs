using Sandbox;
using System.ComponentModel;

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

[Library( "carri" ), AutoGenerate]
public partial class CarriableInfo : ItemInfo
{
	[Property, Category( "Important" )]
	public SlotType Slot { get; set; } = SlotType.Primary;

	[Property, Category( "Important" )]
	public bool Spawnable { get; set; } = false;

	[Property, Category( "Important" )]
	public bool CanDrop { get; set; } = true;

	[Property, Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string ViewModel { get; set; } = "";

	[Property, Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string HandsModel { get; set; } = "";

	[Property, Category( "WorldModels" )]
	public HoldType HoldType { get; set; } = HoldType.None;

	[Property, Category( "WorldModels" ), ResourceType( "vmdl" )]
	public string WorldModel { get; set; } = "";

	[Property, Category( "Stats" )]
	public float DeployTime { get; set; } = 0.6f;

	public Model CachedViewModel { get; private set; }
	public Model CachedWorldModel { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		CachedViewModel = Model.Load( ViewModel );
		CachedWorldModel = Model.Load( WorldModel );
	}
}

public abstract partial class Carriable : BaseCarriable, IEntityHint, IUse
{
	[Net, Local, Predicted]
	public TimeSince TimeSinceDeployed { get; private set; }

	public TimeSince TimeSinceDropped { get; private set; }

	public new Player Owner
	{
		get => base.Owner as Player;
		set => base.Owner = value;
	}

	public BaseViewModel HandsModelEntity { get; private set; }
	public Player PreviousOwner { get; private set; }

	/// <summary>
	/// The text that will show up in the inventory slot.
	/// </summary>
	public virtual string SlotText => string.Empty;
	public CarriableInfo Info { get; private set; }

	public Carriable() { }

	public override void Spawn()
	{
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );

		if ( string.IsNullOrEmpty( ClassInfo?.Name ) )
		{
			Log.Error( this + " doesn't have a Library name!" );
			return;
		}

		Info = Asset.GetInfo<CarriableInfo>( this );
		SetModel( Info.WorldModel );

		Components.Create<DNA>();
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrEmpty( ClassInfo?.Name ) )
			Info = Asset.GetInfo<CarriableInfo>( this );
	}

	public override void ActiveStart( Entity entity )
	{
		EnableDrawing = true;

		if ( entity is Player player )
		{
			var animator = player.GetActiveAnimator();
			if ( animator != null )
			{
				SimulateAnimator( animator );
			}
		}

		if ( IsLocalPawn )
		{
			DestroyViewModel();
			DestroyHudElements();

			CreateViewModel();
			CreateHudElements();

			ViewModelEntity?.SetAnimParameter( "deploy", true );
		}

		TimeSinceDeployed = 0;
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		base.Simulate( client );
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		if ( string.IsNullOrEmpty( Info.ViewModel ) || string.IsNullOrEmpty( Info.HandsModel ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};
		ViewModelEntity.SetModel( Info.ViewModel );

		HandsModelEntity = new BaseViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};
		HandsModelEntity.SetModel( Info.HandsModel );
		HandsModelEntity.SetParent( ViewModelEntity, true );
	}

	public override void CreateHudElements() { }

	public override void DestroyViewModel()
	{
		base.DestroyViewModel();

		HandsModelEntity?.Delete();
		HandsModelEntity = null;
	}

	public override bool CanCarry( Entity carrier )
	{
		if ( Owner is not null || carrier is not Player )
			return false;

		if ( carrier == PreviousOwner && TimeSinceDropped < 1f )
			return false;

		return true;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		PreviousOwner = Owner;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		TimeSinceDropped = 0;
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		base.SimulateAnimator( anim );

		anim.SetAnimParameter( "holdtype", (int)Info.HoldType );
	}

	bool IEntityHint.CanHint( Player player ) => Owner is null;

	bool IUse.OnUse( Entity user )
	{
		if ( user is Player player )
			player.Inventory.Swap( this );

		return false;
	}

	bool IUse.IsUsable( Entity user ) => Owner is null && user is Player;

#if SANDBOX && DEBUG
	[Event.Hotload]
	private void OnHotReload()
	{
		Info = Asset.GetInfo<CarriableInfo>( this );
	}
#endif
}
