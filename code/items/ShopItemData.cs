using System;
using Sandbox;
using TTT.Player;

namespace TTT.Items
{
	public class ShopItemData
	{
		public string Name { get; set; }
		public string Description = "";
		public int Price { get; set; } = 0;
		public SlotType? SlotType = null;
		public Type Type = null;
		public bool IsLimited { get; set; } = true;

		public void CopyFrom( ShopItemData shopItemData )
		{
			Name = shopItemData.Name;
			Price = shopItemData.Price;
			Description = shopItemData.Description ?? Description;
			SlotType = shopItemData.SlotType ?? SlotType;
			Type = shopItemData.Type ?? Type;
			IsLimited = shopItemData.IsLimited;
		}

		public ShopItemData Clone()
		{
			ShopItemData shopItemData = new();
			shopItemData.CopyFrom( this );
			return shopItemData;
		}

		public static ShopItemData CreateItemData( Type type )
		{
			var libraryData = Library.GetAttribute( type );
			var shopData = Utils.GetAttribute<ShopAttribute>( type );
			if ( libraryData == null || shopData == null )
			{
				return null;
			}

			ShopItemData shopItemData = new()
			{
				Name = libraryData.Title,
				Type = type,
				Price = shopData.Price,
				SlotType = shopData.SlotType,
			};

			return shopItemData;
		}

		public bool IsBuyable( TTTPlayer player )
		{
			if ( Type.IsSubclassOf( typeof( TTTPerk ) ) )
			{
				return !player.Inventory.Perks.Has( Name );
			}
			else if ( SlotType == null )
			{
				return false;
			}

			return !player.Inventory.IsCarryingType( Type ) && player.Inventory.HasEmptySlot( SlotType.Value );
		}
	}
}
