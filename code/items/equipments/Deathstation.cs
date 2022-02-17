using System;
using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	[Library( "ttt_equipment_deathstation", Title = "Death Station" )]
	[Shop( SlotType.UtilityEquipment, 100 )]
	[Hammer.Skip]
	public partial class DeathStation : BaseCarriable, ICarriableItem
	{
		public ItemData Data { get; set; }
		public Type DroppedType => typeof( DeathstationEntity );

		public override string ViewModelPath => "";

		public override void Spawn()
		{
			base.Spawn();

			Data = ItemData.All[ClassInfo.Name];
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
					owner.Inventory.DropEntity( this, DroppedType );
				}
			}
		}

		public bool CanDrop() { return false; }
	}
}
