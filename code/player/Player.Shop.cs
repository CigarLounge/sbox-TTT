using Sandbox;
using System.Collections.Generic;

namespace TTT;

public enum BuyError
{
	None,
	InventoryBlocked,
	NotEnoughCredits,
	LimitReached
}

public partial class Player
{
	[Net, Local]
	public IList<string> PurchasedLimitedShopItems { get; set; }

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

	public static void PurchaseItem( string libraryName )
	{
		PurchaseItem( Asset.GetInfo<ItemInfo>( libraryName ).Id );
	}

	[ServerCmd]
	public static void PurchaseItem( int itemId )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.FromId<ItemInfo>( itemId );
		if ( itemInfo is null )
			return;

		if ( !player.Role.Info.AvailableItems.Contains( itemInfo.LibraryName ) )
			return;

		if ( !itemInfo.Buyable || (itemInfo.IsLimited && player.PurchasedLimitedShopItems.Contains( itemInfo.LibraryName )) )
			return;

		if ( player.Credits < itemInfo.Price )
			return;

		if ( itemInfo.IsLimited )
			player.PurchasedLimitedShopItems.Add( itemInfo.LibraryName );

		player.Credits -= itemInfo.Price;
		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( itemInfo.LibraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( itemInfo.LibraryName ) );
	}
}
