using System;
using Sandbox.UI;
namespace TTT.UI;

public partial class ShopItem : Panel
{
	public ItemInfo ItemInfo { get; private set; }
	public bool CanPurchase { get; private set; }

	public ShopItem( ItemInfo item ) => ItemInfo = item;
	public void UpdateAvailability( bool canPurchase ) => CanPurchase = canPurchase;
	protected override int BuildHash() => HashCode.Combine( CanPurchase );
}
