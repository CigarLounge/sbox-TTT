using System;
using Sandbox;

using TTT.Player;

namespace TTT.Items
{
	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = true )]
	public class BuyableAttribute : Attribute
	{
		public int Price = 100;

		public BuyableAttribute() : base()
		{

		}
	}

	[AttributeUsage( AttributeTargets.Class, AllowMultiple = false, Inherited = false )]
	public class Shops : Attribute
	{
		public Type[] Roles;

		public Shops( Type[] roleTypes ) : base()
		{
			Roles = roleTypes;
		}
	}

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
			var isBuyable = Utils.GetAttribute<BuyableAttribute>( type );
			if ( isBuyable == null )
			{
				return null;
			}

			ShopItemData shopItemData = new()
			{
				Name = Utils.GetLibraryTitle( type ),
				Type = type
			};

			var carriable = Utils.GetObjectByType<ICarriableItem>( type );
			if ( carriable != null )
			{
				shopItemData.SlotType = carriable.SlotType;
			}

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
			else if ( Type.IsSubclassOf( typeof( SWB_Base.WeaponBase ) ) )
			{
				return !player.Inventory.IsCarryingType( Type ) && player.Inventory.HasEmptySlot( SlotType.Value );
			}
			else if ( Type.IsSubclassOf( typeof( TTTEquipment ) ) )
			{
				return player.Inventory.HasEmptySlot( SlotType.Value );
			}

			return false;
		}
	}
}
