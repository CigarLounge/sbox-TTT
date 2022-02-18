using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTT.Items;

namespace TTT.Player;

public partial class Inventory : BaseInventory
{
	public new TTTPlayer Owner
	{
		get => (TTTPlayer)base.Owner;
		private init => base.Owner = value;
	}

	public int[] SlotCapacity = new int[] { 1, 1, 1, 3, 3, 1 };

	private const int DROPPOSITIONOFFSET = 50;
	private const int DROPVELOCITY = 500;

	public Inventory( TTTPlayer player ) : base( player ) { }

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

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
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

	public void DropEntity( Entity self, Type entity )
	{
		List.Remove( self );
		self.Delete();

		Entity droppedEntity = Utils.GetObjectByType<Entity>( entity );
		droppedEntity.Position = Owner.EyePosition + Owner.EyeRotation.Forward * DROPPOSITIONOFFSET;
		droppedEntity.Rotation = Owner.EyeRotation;
		droppedEntity.Velocity = Owner.EyeRotation.Forward * DROPVELOCITY;
	}
}
