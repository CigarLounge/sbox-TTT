using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;

using Sandbox;

using TTT.Globals;
using TTT.Items;
using TTT.Roles;
using TTT.Rounds;

namespace TTT.Player
{
	public enum BuyError
	{
		None,
		InventoryBlocked,
		NotEnoughCredits,
		NoAccess,
		NotAvailable,
		LimitReached
	}

	public class Shop
	{
		public List<ShopItemData> Items { set; get; } = new();
		public bool Enabled { get; set; } = true;

		public Shop()
		{

		}

		public bool Accessable()
		{
			return Items.Count > 0 && Enabled && Gamemode.Game.Instance.Round is InProgressRound;
		}

		public static Shop InitializeFromJSON( string json )
		{
			Shop shop = JsonSerializer.Deserialize<Shop>( json );

			if ( shop != null )
			{
				List<ShopItemData> items = new();

				foreach ( ShopItemData shopItemData in shop.Items )
				{
					Type itemType = Utils.GetTypeByLibraryName<IItem>( shopItemData.Name );

					if ( itemType == null || !Utils.HasAttribute<BuyableAttribute>( itemType ) )
					{
						continue;
					}

					// create clean instance
					ShopItemData itemData = ShopItemData.CreateItemData( itemType );

					if ( itemData == null )
					{
						continue;
					}

					// override with settings data
					itemData.CopyFrom( shopItemData );

					items.Add( itemData );
				}

				shop.Items = items;
			}

			return shop;
		}

		public static void Load( TTTRole role )
		{
			string fileName = GetSettingsFile( role );

			if ( !FileSystem.Data.FileExists( fileName ) )
			{
				role.Shop = new();

				role.CreateDefaultShop();
				Utils.CreateRecursiveDirectories( fileName );

				Save( role );

				return;
			}

			role.Shop = Shop.InitializeFromJSON( FileSystem.Data.ReadAllText( fileName ) );

			if ( ShopManager.NewItemsList.Count > 0 )
			{
				role.UpdateDefaultShop( ShopManager.NewItemsList );

				Save( role );
			}
		}

		public static void Save( TTTRole role )
		{
			FileSystem.Data.WriteAllText( GetSettingsFile( role ), JsonSerializer.Serialize( role.Shop, new JsonSerializerOptions
			{
				WriteIndented = true
			} ) );
		}

		public static string GetSettingsFile( TTTRole role )
		{
			return $"settings/{Utils.GetTypeName( typeof( Settings.ServerSettings ) ).ToLower()}/shop/{role.Name.ToLower()}.json";
		}

		internal void AddItemsForRole( TTTRole role )
		{
			Items.Clear();

			foreach ( Type itemType in Utils.GetTypesWithAttribute<IItem, BuyableAttribute>() )
			{
				var itemShopAvailability = itemType.GetCustomAttribute<Shops>();
				if ( itemShopAvailability != null )
				{
					for ( int i = 0; i < itemShopAvailability.Roles.Length; ++i )
					{
						var itemRoleType = itemShopAvailability.Roles[i];
						if ( itemRoleType == role.GetType() )
						{
							Items.Add( ShopItemData.CreateItemData( itemType ) );
						}
					}
				}
			}
		}

		internal void AddNewItems( List<Type> newItemsList )
		{
			List<string> storedItemList = new();

			foreach ( ShopItemData shopItemData in Items )
			{
				storedItemList.Add( Utils.GetLibraryName( shopItemData.Type ) );
			}

			foreach ( Type type in newItemsList )
			{
				bool found = false;
				string newItemName = Utils.GetLibraryName( type );

				foreach ( string storedItemName in storedItemList )
				{
					if ( newItemName.Equals( storedItemName ) )
					{
						found = true;

						break;
					}
				}

				if ( found )
				{
					continue;
				}

				Items.Add( ShopItemData.CreateItemData( type ) );
			}
		}
	}
}
