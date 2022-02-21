using System.Collections.Generic;
using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Local]
	public IList<string> PurchasedShopItems { get; set; }

	[ServerCmd]
	public static void PurchaseItem( string libraryName )
	{
		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() || player.PurchasedShopItems.IsNullOrEmpty() || player.PurchasedShopItems.Contains( libraryName ) )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );
		if ( itemInfo == null )
			return;

		if ( itemInfo.Buyable )
			return;

		// TODO: We need to handle perk here.
		// TODO: we need to handle credits here.
		// TODO: we need to handle the case the inventory is filled.
		player.PurchasedShopItems.Add( libraryName );
		player.Inventory.Add( Library.Create<Carriable>( libraryName ) );
	}
}
