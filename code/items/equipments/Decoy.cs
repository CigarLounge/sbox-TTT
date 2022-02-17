using System;
using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	/// <summary>
	/// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
	/// </summary>
	[Library( "ttt_equipment_decoy", Title = "Decoy" )]
	[Shop( SlotType.UtilityEquipment, 100 )]
	[Hammer.Skip]
	public partial class Decoy : BaseCarriable, ICarriableItem
	{
		public ItemData Data { get; set; }
		public Type DroppedType => typeof( DecoyEntity );

		public override string ViewModelPath => "";

		public override void Spawn()
		{
			base.Spawn();

			Data = ItemData.All[ClassInfo.Name];
		}

		public override void ClientSpawn()
		{
			base.ClientSpawn();

			Data = ItemData.All[ClassInfo.Name];
		}

		public override void Simulate( Client client )
		{
			if ( !IsServer || Owner is not TTTPlayer owner )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					owner.Inventory.DropEntity( this, DroppedType );
				}
			}
		}

		public bool CanDrop() { return false; }
	}
}
