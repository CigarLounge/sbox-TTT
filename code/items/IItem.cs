using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT.Items
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public class ShopAttribute : Attribute
	{
		public SlotType SlotType { get; private set; }
		public int Price { get; private set; }
		public Type[] Roles { get; private set; }

		public ShopAttribute( SlotType slotType, int price = 100, Type[] roles = null ) : base()
		{
			SlotType = slotType;
			Price = price;
			Roles = roles ?? Array.Empty<Type>();
		}
	}

	public class ItemData
	{
		public static Dictionary<string, ItemData> All { get; set; } = new();

		public readonly LibraryData Library;
		public readonly ShopData Shop;

		// Heavily accessed.
		public readonly string Title;
		public readonly SlotType SlotType;

		public ItemData( Type type )
		{
			Library = new LibraryData( type );
			Title = Library.Title;
			Shop = new ShopData( type );
			SlotType = Shop.SlotType;

			All[Library.Name] = this;
		}

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

		public class ShopData
		{
			public SlotType SlotType { get; private set; }
			public int Price { get; private set; }
			public Type[] RoleShopAvailability { get; private set; }

			public ShopData( Type type )
			{
				var shopData = Utils.GetAttribute<ShopAttribute>( type );
				SlotType = shopData.SlotType;
				Price = shopData.Price;
				RoleShopAvailability = shopData.Roles;
			}
		}
	}

	public interface IItem
	{
		static string ITEM_TAG => "TTT_ITEM";
		// public setter, yikes!
		ItemData Data { get; set; }
	}

	public enum SlotType
	{
		Primary = 1,
		Secondary,
		Melee,
		OffensiveEquipment,
		UtilityEquipment,
		Grenade,
		Perk
	}

	public interface ICarriableItem : IItem
	{
		Type DroppedType => null;
		bool CanDrop();
	}
}
