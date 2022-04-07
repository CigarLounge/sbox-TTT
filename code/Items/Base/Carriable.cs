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

	[Property( "viewmodel", title: "View Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string ViewModelPath { get; set; } = "";

	[Property( "handsmodel", title: "Hands Model" ), Category( "ViewModels" ), ResourceType( "vmdl" )]
	public string HandsModelPath { get; set; } = "";

	[Property, Category( "WorldModels" )]
	public HoldType HoldType { get; set; } = HoldType.None;

	[Property( "worldmodel", title: "World Model" ), Category( "WorldModels" ), ResourceType( "vmdl" )]
	public string WorldModelPath { get; set; } = "";

	[Property, Category( "Stats" )]
	public float DeployTime { get; set; } = 0.6f;

	public Model HandsModel { get; private set; }
	public Model ViewModel { get; private set; }
	public Model WorldModel { get; private set; }

	protected override void PostLoad()
	{
		base.PostLoad();

		HandsModel = Model.Load( HandsModelPath );
		ViewModel = Model.Load( ViewModelPath );
		WorldModel = Model.Load( WorldModelPath );
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
	public bool IsActiveChild => Owner?.ActiveChild == this;
	public CarriableInfo Info { get; private set; }

	public Carriable() { }

	public override void Spawn()
	{
		base.Spawn();

		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );

		if ( string.IsNullOrWhiteSpace( ClassInfo?.Name ) )
		{
			Log.Error( this + " doesn't have a Library name!" );
			return;
		}

		Info = Asset.GetInfo<CarriableInfo>( this );
		Model = Info.WorldModel;
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrWhiteSpace( ClassInfo?.Name ) )
			Info = Asset.GetInfo<CarriableInfo>( this );
	}

	public override void ActiveStart( Entity entity )
	{
		EnableDrawing = true;

		if ( entity is Player player )
		{
			var animator = player.GetActiveAnimator();

			if ( animator is not null )
				SimulateAnimator( animator );
		}

		if ( IsLocalPawn )
		{
			CreateViewModel();
			CreateHudElements();

			ViewModelEntity?.SetAnimParameter( "deploy", true );
		}

		TimeSinceDeployed = 0;
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );
	}

	public override void Simulate( Client client ) { }

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );
	}

	public override void CreateViewModel()
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

			ViewModelEntity.Model = Info.ViewModel;
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
