using Sandbox;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public class Inventory : IBaseInventory, IEnumerable<Carriable>
{
	public Player Owner { get; private init; }

	public Entity Active
	{
		get => Owner.ActiveChild;
		set => Owner.ActiveChild = value;
	}

	public Carriable this[int i] => GetSlot( i ) as Carriable;

	private readonly List<Carriable> _list = new();

	private readonly int[] SlotCapacity = new int[] { 1, 1, 1, 3, 3, 1 };
	private readonly int[] WeaponsOfAmmoType = new int[] { 0, 0, 0, 0, 0, 0 };

	private const float DropPositionOffset = 3f;
	private const float DropVelocity = 500f;

	public Inventory( Player player ) => Owner = player;

	public bool Add( Entity entity, bool makeActive = false )
	{
		Host.AssertServer();

		if ( entity.Owner is not null )
			return false;

		if ( !CanAdd( entity ) )
			return false;

		var carriable = (Carriable)entity;
		carriable.Parent = Owner;

		if ( makeActive )
			SetActive( carriable );

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

	public bool Contains( Entity entity ) => _list.Contains( entity );

	public int Count() => _list.Count;

	/// <summary>
	/// Get the item in this slot
	/// </summary>
	public Entity GetSlot( int i )
	{
		if ( _list.Count <= i )
			return null;

		if ( i < 0 )
			return null;

		return _list[i];
	}

	public void Pickup( Entity entity )
	{
		if ( Add( entity ) )
			Sound.FromEntity( Strings.WeaponPickupSound, Owner );
	}

	public bool HasFreeSlot( SlotType slotType )
	{
		return SlotCapacity[(int)slotType] > 0;
	}

	public bool HasWeaponOfAmmoType( AmmoType ammoType )
	{
		return ammoType != AmmoType.None && WeaponsOfAmmoType[(int)ammoType] > 0;
	}

	public void Swap( Carriable carriable )
	{
		Host.AssertServer();

		if ( !carriable.CanCarry( Owner ) )
			return;

		if ( HasFreeSlot( carriable.Info.Slot ) )
		{
			Add( carriable );
			return;
		}

		var entities = _list.FindAll( x => x.Info.Slot == carriable.Info.Slot );
		var active = Active as Carriable;

		if ( active is not null && active.Info.Slot == carriable.Info.Slot )
		{
			if ( DropActive() is not null )
				Add( carriable, true );
		}
		else if ( entities.Count == 1 )
		{
			if ( Drop( entities[0] ) )
				Add( carriable, false );
		}
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
		return _list.Any( x => x.ClassInfo?.Name == libraryName );
	}

	public T Find<T>() where T : Carriable
	{
		foreach ( var carriable in _list )
		{
			if ( carriable is not T t || t.Equals( default( T ) ) )
				continue;

			return t;
		}
		return default;
	}

	public bool Drop( Entity entity )
	{
		if ( !Host.IsServer )
			return false;

		if ( !Contains( entity ) )
			return false;

		var carriable = (Carriable)entity;

		if ( !carriable.Info.CanDrop )
			return false;

		carriable.Parent = null;

		return true;
	}

	public int GetActiveSlot()
	{
		var active = Active;
		int count = Count();

		for ( int i = 0; i < count; i++ )
		{
			if ( _list[i] == active )
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
		Host.AssertServer();

		// Cache due to "collections modified error"
		Active = null;
		foreach ( var carriable in _list.ToArray() )
		{
			Drop( carriable );
		}

		DeleteContents();
	}

	public void DeleteContents()
	{
		Host.AssertServer();

		foreach ( var carriable in _list.ToArray() )
		{
			carriable.Delete();
		}

		_list.Clear();
	}

	public void OnChildAdded( Entity child )
	{
		if ( !CanAdd( child ) )
			return;

		if ( _list.Contains( child ) )
			throw new System.Exception( "Trying to add to inventory multiple times. This is gated by Entity:OnChildAdded and should never happen!" );

		var carriable = (Carriable)child;
		_list.Add( carriable );

		carriable.OnCarryStart( Owner );

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

		if ( !_list.Remove( carriable ) )
			return;

		carriable.OnCarryDrop( Owner );

		if ( !Host.IsServer )
			return;

		SlotCapacity[(int)carriable.Info.Slot] += 1;

		if ( carriable is Weapon weapon )
			WeaponsOfAmmoType[(int)weapon.Info.AmmoType] -= 1;
	}

	public T DropEntity<T>( Entity self ) where T : Entity, new()
	{
		Host.AssertServer();

		var carriable = self as Carriable;
		if ( !carriable.IsValid() || !Contains( carriable ) )
			return null;

		carriable.OnCarryDrop( Owner );
		carriable.Delete();

		var droppedEntity = new T
		{
			Owner = Owner,
			Position = Owner.EyePosition + Owner.EyeRotation.Forward * DropPositionOffset,
			Rotation = Owner.EyeRotation,
			Velocity = Owner.EyeRotation.Forward * DropVelocity
		};

		return droppedEntity;
	}

	public IEnumerator<Carriable> GetEnumerator() => _list.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
