using System;
using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	/// <summary>
	/// Decoy equipment definition, for the physical entity, see items/equipments/entities/DecoyEntity.cs
	/// </summary>
	[Library( "ttt_equipment_decoy", Title = "Decoy" )]
	[Hammer.Skip]
	public partial class Decoy : BaseCarriable, ICarriableItem
	{
		public override string ViewModelPath => "";
		public SlotType SlotType => SlotType.UtilityEquipment;
		public Type DroppedType => typeof( DecoyEntity );
		private readonly LibraryData _data = new( typeof( Decoy ) );

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

		public LibraryData GetLibraryData() { return _data; }
		public bool CanDrop() { return false; }
	}
}
