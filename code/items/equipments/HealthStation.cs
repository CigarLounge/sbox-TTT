using System;
using Sandbox;

using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
	[Shops( new Type[] { typeof( DetectiveRole ) } )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class HealthStation : BaseCarriable, ICarriableItem
	{
		public override string ViewModelPath => "";
		public SlotType SlotType => SlotType.UtilityEquipment;
		private readonly ItemData _data = new( typeof( HealthStation ) );

		public override void Simulate( Client client )
		{
			if ( Owner is not TTTPlayer owner || !IsServer )
			{
				return;
			}

			using ( Prediction.Off() )
			{
				if ( Input.Pressed( InputButton.Attack1 ) )
				{
					owner.Inventory.DropEntity( this, typeof( HealthstationEntity ) );
				}
			}
		}

		public ItemData GetItemData() { return _data; }
		public bool CanDrop() { return false; }
	}
}
