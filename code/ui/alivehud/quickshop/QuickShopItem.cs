using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class QuickShopItem : Panel
{
	public bool IsDisabled = false;

	private readonly Image _itemIcon;
	private readonly Label _itemNameLabel;
	private readonly Label _itemPriceLabel;

	public QuickShopItem( Panel parent ) : base( parent )
	{
		AddClass( "rounded" );
		AddClass( "text-shadow" );
		AddClass( "background-color-primary" );

		_itemPriceLabel = Add.Label();
		_itemPriceLabel.AddClass( "item-price-label" );
		_itemPriceLabel.AddClass( "text-color-info" );

		_itemIcon = Add.Image();
		_itemIcon.AddClass( "item-icon" );

		_itemNameLabel = Add.Label();
		_itemNameLabel.AddClass( "item-name-label" );
	}

	public void InitItem( ItemInfo itemInfo )
	{
		_itemNameLabel.Text = itemInfo.Title;
		_itemPriceLabel.Text = $"${itemInfo.Price}";

		_itemIcon.SetImage( itemInfo.Icon );
	}

	public void Update()
	{
		QuickShop.BuyError buyError = QuickShop.BuyError.None;

		// Decrease the opacity to show that the item cannot be purchased
		// ex. lack of credits
		SetClass( "cannot-purchase", buyError != QuickShop.BuyError.None );
	}
}
