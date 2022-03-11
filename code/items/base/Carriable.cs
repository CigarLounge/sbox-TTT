using Sandbox;
using System;
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
	public Model CachedViewModel { get; set; }

	[Property, Category( "Important" )] public SlotType Slot { get; set; }
	[Property, Category( "Important" )] public bool Spawnable { get; set; }
	[Property, Category( "Important" )] public bool CanDrop { get; set; } = true;
	[Property, Category( "ViewModels" ), ResourceType( "vmdl" )] public string ViewModel { get; set; } = "";
	[Property, Category( "ViewModels" ), ResourceType( "vmdl" )] public string HandsModel { get; set; } = "";
	[Property, Category( "WorldModels" )] public HoldType HoldType { get; set; }
	[Property, Category( "Stats" )] public float DeployTime { get; set; } = 0.6f;

	protected override void PostLoad()
	{
		base.PostLoad();

		CachedViewModel = Model.Load( ViewModel );
	}
}

public abstract partial class Carriable : BaseCarriable, IEntityHint, IUse
{
	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; private set; }

	public TimeSince TimeSinceDropped { get; private set; }

	public new Player Owner
	{
		get => base.Owner as Player;
		set => base.Owner = value;
	}
	public Player PreviousOwner { get; set; }

	public CarriableInfo Info => Asset.GetInfo<CarriableInfo>( ClassInfo.Name );
	string IEntityHint.TextOnTick => Info.Title;
	public BaseViewModel HandsModelEntity;

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

		SetModel( Info.WorldModel );
	}

	public override void ActiveStart( Entity ent )
	{
		EnableDrawing = true;

		if ( ent is Player player )
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

	public override void Simulate( Client cl )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		base.Simulate( cl );
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
		if ( Owner != null || carrier is not Player player )
			return false;

		if ( carrier == PreviousOwner && TimeSinceDropped < 1f )
			return false;

		if ( !player.Inventory.HasFreeSlot( Info.Slot ) )
			return false;

		return true;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		PreviousOwner = Owner;
		Owner.Inventory.SlotCapacity[(int)Info.Slot]--;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		TimeSinceDropped = 0;
		PreviousOwner.Inventory.SlotCapacity[(int)Info.Slot]++;
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		base.SimulateAnimator( anim );

		anim.SetAnimParameter( "holdtype", (int)Info.HoldType );
	}

	public float HintDistance => Player.INTERACT_DISTANCE;

	bool IEntityHint.CanHint( Player player )
	{
		return Owner == null;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Hint( (this as IEntityHint).TextOnTick );
	}

	void IEntityHint.Tick( Player player ) { }

	bool IUse.OnUse( Entity user )
	{
		if ( IsServer && user is Player player )
			player.Inventory.Swap( this );

		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		if ( Owner != null || user is not Player )
			return false;

		return true;
	}
}
