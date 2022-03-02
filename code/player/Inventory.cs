using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTT;

public partial class Inventory : BaseInventory
{
	public new Player Owner
	{
		get => base.Owner as Player;
		private init => base.Owner = value;
	}

	public int[] SlotCapacity = new int[] { 1, 1, 1, 3, 3, 1 };
	public int[] WeaponsOfAmmoType = new int[] { 0, 0, 0, 0, 0, 0 }; 

	static readonly List<int> AmmoLimits = new()
	{
		[(int)AmmoType.None] = 0,
		[(int)AmmoType.PistolSMG] = 60,
		[(int)AmmoType.Shotgun] = 16,
		[(int)AmmoType.Sniper] = 20,
		[(int)AmmoType.Magnum] = 12,
		[(int)AmmoType.Rifle] = 40
	};

	private const int DROPPOSITIONOFFSET = 50;
	private const int DROPVELOCITY = 500;

	public Inventory( Player player ) : base( player ) { }

	public override void Pickup( Entity entity )
	{
		if ( base.Add( entity ) )
			Sound.FromEntity( RawStrings.WeaponPickupSound, Owner );
	}

	public bool HasFreeSlot( SlotType slotType )
	{
		return SlotCapacity[(int)slotType] > 0;
	}

	public bool HasWeaponOfAmmoType( AmmoType ammoType )
	{
		return ammoType != AmmoType.None && WeaponsOfAmmoType[(int)ammoType] > 0;
	}

	public static int GetAmmoLimit( AmmoType ammoType )
	{
		return AmmoLimits.ElementAtOrDefault( ammoType, 0 );
	}

	public Carriable Swap( Carriable carriable )
	{
		var ent = List.Find( x => (x as Carriable).Info.Slot == carriable.Info.Slot );
		bool wasActive = Owner.ActiveChild == ent;

		if ( ent.IsValid() )
			Drop( ent );

		Add( carriable, wasActive );

		return ent as Carriable;
	}

	public bool IsCarrying( string libraryName )
	{
		return List.Any( x => x.ClassInfo?.Name == libraryName );
	}

	public override bool Drop( Entity ent )
	{
		var carriableInfo = Asset.GetInfo<CarriableInfo>( ent );
		if ( !carriableInfo.CanDrop )
			return false;

		return base.Drop( ent );
	}

	public void DropAll()
	{
		// Cache due to "collections modified error"
		Active = null;
		List<Entity> cache = new( List );
		foreach ( Entity entity in cache )
		{
			Drop( entity );
		}
	}

	public override void DeleteContents()
	{
		Host.AssertServer();

		foreach ( var item in List.ToArray() )
		{
			item.OnChildRemoved( Owner );
			SlotCapacity[(int)(item as Carriable).Info.Slot]++;
			if ( item is Weapon weapon )
				WeaponsOfAmmoType[(int)weapon.Info.AmmoType]--;
			item.Delete();
		}

		List.Clear();
	}

	public void DropEntity( Entity self, Type type )
	{
		List.Remove( self );
		self.Delete();

		Entity droppedEntity = Library.Create<Entity>( type );
		droppedEntity.Position = Owner.EyePosition + Owner.EyeRotation.Forward * DROPPOSITIONOFFSET;
		droppedEntity.Rotation = Owner.EyeRotation;
		droppedEntity.Velocity = Owner.EyeRotation.Forward * DROPVELOCITY;
	}
}
