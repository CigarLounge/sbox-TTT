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
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );
		if ( itemInfo == null )
			return;

		if ( !player.Role.Info.AvailableItems.Contains( libraryName ) )
			return;

		if ( !itemInfo.Buyable || (itemInfo.IsLimited && player.PurchasedShopItems.Contains( libraryName )) )
			return;

		if ( player.Credits < itemInfo.Price )
			return;

		player.Credits -= itemInfo.Price;
		player.PurchasedShopItems.Add( libraryName );
		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( libraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( libraryName ) );
	}
}
