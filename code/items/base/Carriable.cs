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

[Library( "carri" ), AutoGenerate]
public partial class CarriableInfo : ItemInfo
{
	public Model CachedViewModel { get; set; }

	[Property, Category( "Important" )] public SlotType Slot { get; set; }
	[Property, Category( "Important" )] public bool Spawnable { get; set; }
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string ViewModel { get; set; } = "";
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string HandsModel { get; set; } = "";
	[Property, Category( "Stats" )] public float DeployTime { get; set; } = 0.6f;

	protected override void PostLoad()
	{
		base.PostLoad();

		CachedViewModel = Model.Load( ViewModel );
		CachedWorldModel = Model.Load( WorldModel );
	}
}

[Hammer.Skip]
public abstract partial class Carriable : BaseCarriable, IEntityHint
{
	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	public new Player Owner
	{
		get => base.Owner as Player;
		set => base.Owner = value;
	}

	public CarriableInfo Info { get; set; }
	string IEntityHint.TextOnTick => throw new NotImplementedException();
	public BaseViewModel HandsModelEntity;

	public Carriable() { }

	public override void Spawn()
	{
		base.Spawn();

		// TODO: @mzegar
		CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
		SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

		if ( string.IsNullOrEmpty( ClassInfo?.Name ) )
		{
			Log.Error( this + " doesn't have a Library name!" );

			return;
		}

		Info = Asset.GetInfo<CarriableInfo>( ClassInfo.Name );
		SetModel( Info.WorldModel );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrEmpty( ClassInfo.Name ) )
			Info = Asset.GetInfo<CarriableInfo>( ClassInfo?.Name );
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
			Owner = Owner,
			EnableViewmodelRendering = true
		};
		HandsModelEntity.SetModel( Info.HandsModel );
		HandsModelEntity.SetParent( ViewModelEntity, true );
	}

	public override void CreateHudElements() { }

	public override bool CanCarry( Entity carrier )
	{
		if ( Owner != null || carrier is not Player player )
			return false;

		// TODO: @mzegar
		//if ( Info.ExclusiveFor != Team.None && player.Team != Info.ExclusiveFor )
		//return false;

		if ( !player.Inventory.HasFreeSlot( Info.Slot ) )
			return false;

		return true;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		Owner.Inventory.SlotCapacity[(int)Info.Slot]--;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		(dropper as Player).Inventory.SlotCapacity[(int)Info.Slot]++;
	}

	bool IEntityHint.CanHint( Player player )
	{
		throw new NotImplementedException();
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		throw new NotImplementedException();
	}

	void IEntityHint.Tick( Player player )
	{
		throw new NotImplementedException();
	}
}
