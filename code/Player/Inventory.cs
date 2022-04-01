using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public class Inventory : IBaseInventory
{
	public Player Owner { get; init; }

	public Entity Active
	{
		get => Owner.ActiveChild;
		set => Owner.ActiveChild = value;
	}

	public List<Carriable> List { get; set; } = new();

	public int[] SlotCapacity = new int[] { 1, 1, 1, 3, 3, 1 };
	public int[] WeaponsOfAmmoType = new int[] { 0, 0, 0, 0, 0, 0 };

	private const int DROPPOSITIONOFFSET = 50;
	private const int DROPVELOCITY = 500;

	public Inventory( Player player ) => Owner = player;

	public bool Add( Entity entity, bool makeActive = false )
	{
		Host.AssertServer();

		if ( entity.Owner is not null )
			return false;

		if ( !CanAdd( entity ) )
			return false;

		var carriable = entity as Carriable;
		carriable.SetParent( Owner );
		carriable.OnCarryStart( Owner );

		if ( makeActive )
			SetActive( entity );

		return true;
	}

	public bool CanAdd( Entity entity )
	{
		if ( entity is not Carriable carriable )
			return false;

		if ( Host.IsClient )
			return true;

		if ( !HasFreeSlot( carriable.Info.Slot ) )
			return false;

		if ( !carriable.CanCarry( Owner ) )
			return false;

		return true;
	}

	public bool Contains( Entity entity ) => List.Contains( entity );

	public int Count() => List.Count;

	/// <summary>
	/// Get the item in this slot
	/// </summary>
	public Entity GetSlot( int i )
	{
		if ( List.Count <= i )
			return null;

		if ( i < 0 )
			return null;

		return List[i];
	}

	public void Pickup( Entity entity )
	{
		if ( Add( entity ) )
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

	public Carriable Swap( Carriable carriable )
	{
		var entity = List.Find( x => x.Info.Slot == carriable.Info.Slot );
		bool wasActive = Owner.ActiveChild == entity;

		if ( entity.IsValid() )
			Drop( entity );

		Add( carriable, wasActive );

		return entity;
	}

	public bool SetActive( Entity entity )
	{
		if ( Active == entity )
			return false;

		if ( !Contains( entity ) )
			return false;

		Active = entity;
		return true;
	}

	public bool IsCarrying( string libraryName )
	{
		return List.Any( x => x.ClassInfo?.Name == libraryName );
	}

	public bool Drop( Entity entity )
	{
		if ( !Host.IsServer )
			return false;

		if ( !Contains( entity ) )
			return false;

		var carriable = entity as Carriable;
		carriable.Parent = null;
		carriable.OnCarryDrop( Owner );

		return true;
	}

	public int GetActiveSlot()
	{
		var active = Active;
		int count = Count();

		for ( int i = 0; i < count; i++ )
		{
			if ( List[i] == active )
				return i;
		}

		return -1;
	}

	public Entity DropActive()
	{
		if ( !Host.IsServer )
			return null;

		var active = Active;
		if ( active is not Carriable carriable )
			return null;

		if ( !carriable.Info.CanDrop )
			return null;

		if ( Drop( carriable ) )
		{
			Active = null;
			return carriable;
		}

		return null;
	}

	public bool SetActiveSlot( int i, bool evenIfEmpty = false )
	{
		var entity = GetSlot( i );
		if ( Active == entity )
			return false;

		if ( !evenIfEmpty && entity is null )
			return false;

		Active = entity;
		return entity.IsValid();
	}

	public bool SwitchActiveSlot( int idelta, bool loop )
	{
		int count = Count();
		if ( count == 0 )
			return false;

		int slot = GetActiveSlot();
		int nextSlot = slot + idelta;

		if ( loop )
		{
			while ( nextSlot < 0 )
				nextSlot += count;
			while ( nextSlot >= count )
				nextSlot -= count;
		}
		else
		{
			if ( nextSlot < 0 )
				return false;
			if ( nextSlot >= count )
				return false;
		}

		return SetActiveSlot( nextSlot, false );
	}


	public void DropAll()
	{
		// Cache due to "collections modified error"
		Active = null;
		foreach ( var carriable in List.ToArray() )
		{
			Drop( carriable );
		}
	}

	public void DeleteContents()
	{
		Host.AssertServer();

		foreach ( var item in List.ToArray() )
		{
			item.Delete();
		}

		List.Clear();
	}

	public void OnChildAdded( Entity child )
	{
		if ( !CanAdd( child ) )
			return;

		if ( List.Contains( child ) )
			throw new System.Exception( "Trying to add to inventory multiple times. This is gated by Entity:OnChildAdded and should never happen!" );

		var carriable = child as Carriable;
		List.Add( carriable );

		if ( !Host.IsServer )
			return;

		SlotCapacity[(int)carriable.Info.Slot] -= 1;
		if ( carriable is Weapon weapon )
			WeaponsOfAmmoType[(int)weapon.Info.AmmoType] += 1;
	}

	public void OnChildRemoved( Entity child )
	{
		if ( child is not Carriable carriable )
			return;

		if ( List.Remove( carriable ) )
		{
			SlotCapacity[(int)carriable.Info.Slot] += 1;
			if ( carriable is Weapon weapon )
				WeaponsOfAmmoType[(int)weapon.Info.AmmoType] -= 1;
		}
	}

	public Entity DropEntity( Entity self, Entity droppedEntity )
	{
		if ( !Drop( self ) )
		{
			droppedEntity.Delete();
			return null;
		}

		self.Delete();

		droppedEntity.Position = Owner.EyePosition + Owner.EyeRotation.Forward * DROPPOSITIONOFFSET;
		droppedEntity.Rotation = Owner.EyeRotation;
		droppedEntity.Velocity = Owner.EyeRotation.Forward * DROPVELOCITY;
		droppedEntity.Owner = Owner;

		return droppedEntity;
	}
}
