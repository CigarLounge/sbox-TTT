using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTT.Items;

namespace TTT.Player
{
	public partial class Inventory : BaseInventory
	{
		public new TTTPlayer Owner
		{
			get => (TTTPlayer)base.Owner;
			private init => base.Owner = value;
		}

		public readonly int[] SlotCapacity = new int[] { 1, 1, 1, 3, 3, 1 };

		private const int DROPPOSITIONOFFSET = 50;
		private const int DROPVELOCITY = 500;

		public Inventory( TTTPlayer player ) : base( player ) { }

		// This code is poorly written, we should fix this.
		public override bool Add( Entity entity, bool makeActive = false )
		{
			if ( entity is not IItem item || IsCarryingType( entity.GetType() ) || !HasEmptySlot( item.Data.SlotType ) || !base.Add( entity, makeActive ) )
			{
				return false;
			}

			Sound.FromWorld( "dm.pickup_weapon", entity.Position );
			return true;
		}

		public bool HasEmptySlot( SlotType slotType )
		{
			int itemsInSlot = List.Count( x => ((IItem)x).Data.SlotType == slotType );
			return SlotCapacity[(int)slotType - 1] - itemsInSlot > 0;
		}

		public bool IsCarryingType( Type t )
		{
			return List.Any( x => x.GetType() == t );
		}

		public override bool Drop( Entity entity )
		{
			if ( entity is not ICarriableItem item || !item.CanDrop() )
			{
				return false;
			}

			return base.Drop( entity );
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
			droppedEntity.Tags.Add( IItem.ITEM_TAG );
		}
	}
}
