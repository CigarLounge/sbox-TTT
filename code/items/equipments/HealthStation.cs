using System;
using Sandbox;

using TTT.Player;
using TTT.Roles;

namespace TTT.Items
{
	[Library( "ttt_equipment_healthstation", Title = "Health Station" )]
	[Equipment( SlotType = SlotType.UtilityEquipment )]
	[Shops( new Type[] { typeof( DetectiveRole ) } )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class HealthStation : TTTEquipment
	{
		public override string ViewModelPath => "";

		public override void Spawn()
		{
			base.Spawn();

			RenderColor = Color.Transparent;
		}

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

		public override bool CanDrop() => false;
	}
}
