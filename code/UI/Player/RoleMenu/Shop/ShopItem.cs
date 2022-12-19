using System;
using Sandbox.UI;
namespace TTT.UI;

public partial class ShopItem : Panel
{
	public ItemInfo ItemInfo { get; set; }
	public bool CanPurchase { get; set; }
	public void UpdateAvailability( bool canPurchase ) => CanPurchase = canPurchase;
	protected override int BuildHash() => HashCode.Combine( CanPurchase );
}
