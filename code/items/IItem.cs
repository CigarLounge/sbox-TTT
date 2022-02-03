using System;


namespace TTT.Items
{
	public class ItemData
	{
		public string LibraryName { get; private set; }
		public string LibraryTitle { get; private set; }

		public ItemData( Type type )
		{
			LibraryName = Utils.GetLibraryName( type );
			LibraryName = Utils.GetLibraryTitle( type );
		}
	}

	public interface IItem
	{
		static string ITEM_TAG => "TTT_ITEM";
		ItemData GetItemData();
		void Delete();
	}

	public enum SlotType
	{
		Primary = 1,
		Secondary,
		Melee,
		OffensiveEquipment,
		UtilityEquipment,
		Grenade
	}

	public interface ICarriableItem : IItem
	{
		SlotType SlotType { get; }
		Type DroppedType => null;
		bool CanDrop();
	}
}
