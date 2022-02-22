using System.Collections.Generic;
using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Local]
	public IList<string> PurchasedLimitedShopItems { get; set; }

	public enum BuyError
	{
		None,
		InventoryBlocked,
		NotEnoughCredits,
		LimitReached
	}

	/// <summary>
	/// Local clientside check before server does exact same checks in "PurchaseItem(...)".
	/// IsLimited check 
	/// </summary>
	public BuyError CanPurchase( ItemInfo item )
	{
		if ( Credits < item.Price )
			return BuyError.NotEnoughCredits;

		if ( PurchasedLimitedShopItems.Contains( item.LibraryName ) )
			return BuyError.LimitReached;

		if ( item is CarriableInfo carriable && !Inventory.HasFreeSlot( carriable.Slot ) )
			return BuyError.InventoryBlocked;

		return BuyError.None;
	}

	[ServerCmd]
	public static void PurchaseItem( string libraryName )
	{
		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );
		if ( itemInfo == null )
			return;

		if ( !player.Role.Info.AvailableItems.Contains( libraryName ) )
			return;

		if ( !itemInfo.Buyable || (itemInfo.IsLimited && player.PurchasedLimitedShopItems.Contains( libraryName )) )
			return;

		if ( player.Credits < itemInfo.Price )
			return;

		if ( itemInfo.IsLimited )
			player.PurchasedLimitedShopItems.Add( libraryName );

		player.Credits -= itemInfo.Price;
		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( libraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( libraryName ) );
	}
}
