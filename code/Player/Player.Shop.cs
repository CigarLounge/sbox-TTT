using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	[Net, Local]
	public int Credits { get; set; }

	[Net, Local]
	public IList<ItemInfo> PurchasedLimitedShopItems { get; private set; }

	public bool CanPurchase( ItemInfo item )
	{
		if ( Credits < item.Price )
			return false;

		if ( !Role.ShopItems.Contains( item ) )
			return false;

		if ( item is CarriableInfo carriable && !Inventory.HasFreeSlot( carriable.Slot ) )
			return false;

		if ( item.IsLimited && PurchasedLimitedShopItems.Contains( item ) )
			return false;

		return true;
	}

	[ConCmd.Server]
	public static void PurchaseItem( int itemId )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = ResourceLibrary.Get<ItemInfo>( itemId );
		if ( itemInfo is null )
			return;

		if ( !player.CanPurchase( itemInfo ) )
			return;

		if ( itemInfo.IsLimited )
			player.PurchasedLimitedShopItems.Add( itemInfo );

		player.Credits -= itemInfo.Price;

		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( TypeLibrary.Create<Carriable>( itemInfo.ClassName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( TypeLibrary.Create<Perk>( itemInfo.ClassName ) );
	}
}
