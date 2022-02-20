using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTT.Events;
using TTT.Globals;
using TTT.Items;
using TTT.Rounds;
using TTT.UI;

namespace TTT.Player
{
	public partial class TTTPlayer
	{
		public HashSet<string> BoughtItemsSet = new();

		public Shop Shop
		{
			get => _shop;
			set
			{
				if ( _shop == value )
				{
					return;
				}

				_shop = value;

				Event.Run( TTTEvent.Shop.Change );
			}
		}
		private Shop _shop;

		public BuyError CanBuy( ShopItemData itemData )
		{
			if ( Shop == null || !Shop.Accessable() )
			{
				return BuyError.NoAccess;
			}

			if ( !itemData?.IsBuyable( this ) ?? false )
			{
				return BuyError.InventoryBlocked;
			}

			if ( Credits < itemData?.Price )
			{
				return BuyError.NotEnoughCredits;
			}

			bool found = false;

			foreach ( ShopItemData shopItemData in Shop.Items )
			{
				if ( shopItemData.Name.Equals( itemData.Name ) )
				{
					found = true;

					break;
				}
			}

			if ( !found )
			{
				return BuyError.NotAvailable;
			}

			return BuyError.None;
		}

		public void RequestPurchase( Type itemType )
		{
			ShopItemData itemData = ShopItemData.CreateItemData( itemType );

			foreach ( ShopItemData loopItemData in Shop.Items )
			{
				if ( loopItemData.Name.Equals( itemData.Name ) )
				{
					itemData.CopyFrom( loopItemData );
				}
			}

			BuyError buyError = CanBuy( itemData );

			if ( buyError != BuyError.None )
			{
				Log.Warning( $"{Client.Name} tried to buy '{itemData.Name}'. (Error: {buyError})" );

				return;
			}

			Credits -= itemData.Price;

			AddItem( Utils.GetObjectByType<IItem>( itemType ) );
			BoughtItemsSet.Add( itemData.Name );

			ClientBoughtItem( To.Single( this ), itemData.Name );
		}

		[Event( TTTEvent.Game.RoundChange )]
		private static void OnRoundChanged( BaseRound oldRound, BaseRound newRound )
		{
			if ( newRound is PreRound preRound )
			{
				foreach ( TTTPlayer player in Utils.GetPlayers() )
				{
					player.BoughtItemsSet.Clear();
				}
			}
		}

		[ClientRpc]
		public static void ClientBoughtItem( string itemName )
		{
			(Local.Pawn as TTTPlayer).BoughtItemsSet.Add( itemName );

			UpdateQuickShop();
		}

		[ClientRpc]
		public static void ClientSendQuickShopUpdate()
		{
			UpdateQuickShop();
		}

		private static void UpdateQuickShop()
		{
			if ( QuickShop.Instance?.Enabled ?? false )
			{
				QuickShop.Instance.Update();
			}
		}

		public void ServerUpdateShop()
		{
			ClientUpdateShop( To.Single( this ), JsonSerializer.Serialize( Shop ) );
		}

		[ClientRpc]
		public static void ClientUpdateShop( string shopJson )
		{
			(Local.Pawn as TTTPlayer).Shop = Shop.InitializeFromJSON( shopJson );
		}
	}
}
