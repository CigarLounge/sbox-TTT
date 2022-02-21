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

	private const int DROPPOSITIONOFFSET = 50;
	private const int DROPVELOCITY = 500;

	public Inventory( Player player ) : base( player ) { }

	public override void Pickup( Entity entity )
	{
		// TODO: Play some sound on pickup.
		// Sound.FromEntity( "pickup_weapon", Owner );
		if ( base.Add( entity, Active == null ) ) { }
	}

	public bool HasFreeSlot( SlotType slotType )
	{
		return SlotCapacity[(int)slotType] > 0;
	}

	public Carriable Swap( Carriable carriable )
	{
		var ent = List.Find( x => (x as Carriable).Info.Slot == carriable.Info.Slot );
		bool wasActive = ent?.IsActiveChild() ?? false;

		Drop( ent );
		Add( carriable, wasActive );

		return ent as Carriable;
	}

	public bool IsCarrying( string libraryName )
	{
		return List.Any( x => x.ClassInfo?.Name == libraryName );
	}

	public void DropAll()
	{
		// Cache due to "collections modified error"
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
			item.OnCarryDrop( Owner );
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
