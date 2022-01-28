using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	/// <summary>
	/// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
	/// </summary>
	[Library( "Decoy" )]
	[Weapon( SlotType = SlotType.UtilityEquipment )]
	[Buyable( Price = 100 )]
	[Hammer.Skip]
	public partial class DecoyEquipment : TTTEquipment
	{
		public override string ViewModelPath => "";

		public DecoyEquipment() : base()
		{

		}

		public override bool CanDrop() => false;

		public override void Spawn()
		{
			base.Spawn();

			RenderColor = Color.Transparent;
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
					owner.Inventory.DropEntity( this, typeof( DecoyEntity ) );
				}
			}
		}
	}
}
