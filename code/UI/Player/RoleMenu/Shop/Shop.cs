using System;
using Sandbox;
using Sandbox.UI;
namespace TTT.UI;

public partial class Shop : Panel
{
	private ItemInfo _selectedItem;

	protected override int BuildHash()
	{
		return HashCode.Combine(
			(Game.LocalPawn as Player)?.Credits,
			(Game.LocalPawn as Player)?.Role?.ShopItems,
			_selectedItem
		);
	}
}
