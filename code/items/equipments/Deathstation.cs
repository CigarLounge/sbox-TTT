using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	[Library( "Death Station" )]
	[Equipment( SlotType = SlotType.UtilityEquipment )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class DeathStation : TTTEquipment
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
					owner.Inventory.DropEntity( this, typeof( DeathstationEntity ) );
				}
			}
		}

		public override bool CanDrop() => false;
	}
}
