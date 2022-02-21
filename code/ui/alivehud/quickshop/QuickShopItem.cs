using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class QuickShopItem : Panel
{
	public bool IsDisabled { get; private set; }

	private readonly Image _itemIcon;
	private readonly Label _itemNameLabel;
	private readonly Label _itemPriceLabel;

	public QuickShopItem( Panel parent, ItemInfo itemInfo ) : base( parent )
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

		_itemNameLabel.Text = itemInfo.Title;
		_itemPriceLabel.Text = $"${itemInfo.Price}";
		_itemIcon.SetImage( itemInfo.Icon );
	}

	public void UpdateAvailability( bool isDisabled )
	{
		SetClass( "cannot-purchase", isDisabled );
		IsDisabled = isDisabled;
	}
}
