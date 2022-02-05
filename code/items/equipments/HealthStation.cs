using System;
using Sandbox;

using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
	[Shop( SlotType.UtilityEquipment, 100, new Type[] { typeof( DetectiveRole ) } )]
	[Hammer.Skip]
	public partial class HealthStation : BaseCarriable, ICarriableItem
	{
		public ItemData GetItemData() { return _data; }
		private readonly ItemData _data = new( typeof( HealthStation ) );

		public override string ViewModelPath => "";

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

		public bool CanDrop() { return false; }
	}
}
