using Sandbox;
using Sandbox.player;
using System;
using System.ComponentModel;
using TTT.Player;
using TTT.UI;

namespace TTT.Items;

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
	public Model CachedWorldModel { get; set; }

	[Property, Category( "Important" )] public bool Buyable { get; set; }
	[Property, Category( "Important" )] public SlotType Slot { get; set; }
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string ViewModel { get; set; } = "";
	[Property, Category( "Models" ), ResourceType( "vmdl" )] public string WorldModel { get; set; } = "";
	[Property, Category( "Stats" )] public float DeployTime { get; set; } = 0.6f;
	[Property, Category( "Stats" )] public int Price { get; set; }

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

	public CarriableInfo Info { get; set; }
	public PickupTrigger PickupTrigger { get; protected set; }
	public new TTTPlayer Owner
	{
		get => base.Owner as TTTPlayer;
		set => base.Owner = value;
	}
	string IEntityHint.TextOnTick => throw new NotImplementedException();

	public Carriable() { }

	public override void Spawn()
	{
		base.Spawn();

		// TODO: @mzegar
		CollisionGroup = CollisionGroup.Weapon; // so players touch it as a trigger but not as a solid
		SetInteractsAs( CollisionLayer.Debris ); // so player movement doesn't walk into it

		PickupTrigger = new PickupTrigger
		{
			Parent = this,
			Position = Position
		};

		PickupTrigger.PhysicsBody.EnableAutoSleeping = false;

		if ( string.IsNullOrEmpty( ClassInfo?.Name ) )
		{
			Log.Error( this + " doesn't have a Library name!" );

			return;
		}

		Info = ItemInfo.All[ClassInfo.Name] as CarriableInfo;
		SetModel( Info.WorldModel );
	}

	public override void ClientSpawn()
	{
		base.ClientSpawn();

		if ( !string.IsNullOrEmpty( ClassInfo.Name ) )
			Info = CarriableInfo.All[ClassInfo?.Name] as CarriableInfo;
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

		if ( string.IsNullOrEmpty( Info.ViewModel ) )
			return;

		ViewModelEntity = new ViewModel
		{
			Position = Position,
			Owner = Owner,
			EnableViewmodelRendering = true
		};

		ViewModelEntity.SetModel( Info.ViewModel );
	}

	public override void CreateHudElements() { }

	public override bool CanCarry( Entity carrier )
	{
		if ( Owner != null || carrier is not TTTPlayer player )
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

		if ( PickupTrigger.IsValid() )
			PickupTrigger.EnableTouch = false;

		Owner.Inventory.SlotCapacity[(int)Info.Slot]--;
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		if ( PickupTrigger.IsValid() )
			PickupTrigger.EnableTouch = true;

		(dropper as TTTPlayer).Inventory.SlotCapacity[(int)Info.Slot]++;
	}

	bool IEntityHint.CanHint( TTTPlayer player )
	{
		throw new NotImplementedException();
	}

	EntityHintPanel IEntityHint.DisplayHint( TTTPlayer player )
	{
		throw new NotImplementedException();
	}

	void IEntityHint.Tick( TTTPlayer player )
	{
		throw new NotImplementedException();
	}
}
