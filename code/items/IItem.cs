using System;
using System.Collections.Generic;
using TTT.Roles;

namespace TTT.Items
{
	public class LibraryData
	{
		public string Name { get; private set; }
		public string Title { get; private set; }

		public LibraryData( Type type )
		{
			Name = Utils.GetLibraryName( type );
			Title = Utils.GetLibraryTitle( type );
		}
	}

	public interface IItem
	{
		static string ITEM_TAG => "TTT_ITEM";
		List<TTTRole> ShopAvailability => new();
		int Price => 0;
		LibraryData GetLibraryData();
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
