using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Items;
using TTT.Player;

namespace TTT.UI;

public partial class QuickShopItem : Panel
{
	public ShopItemData ItemData;
	public bool IsDisabled = false;

	private Panel _itemIcon;
	private Label _itemNameLabel;
	private Label _itemPriceLabel;

	public QuickShopItem( Panel parent ) : base( parent )
	{
		AddClass( "rounded" );
		AddClass( "text-shadow" );
		AddClass( "background-color-primary" );

		_itemPriceLabel = Add.Label();
		_itemPriceLabel.AddClass( "item-price-label" );
		_itemPriceLabel.AddClass( "text-color-info" );

		_itemIcon = new Panel( this );
		_itemIcon.AddClass( "item-icon" );

		_itemNameLabel = Add.Label();
		_itemNameLabel.AddClass( "item-name-label" );
	}

	public void SetItem( ShopItemData shopItemData )
	{
		ItemData = shopItemData;

		_itemNameLabel.Text = shopItemData.Name;
		_itemPriceLabel.Text = $"${shopItemData.Price}";

		_itemIcon.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, $"/ui/icons/{shopItemData.Name}.png", false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public void Update()
	{
		BuyError buyError = (Local.Pawn as TTTPlayer).CanBuy( ItemData );

		// Let's not show any items that the player could not access in the first place.
		SetClass( "disabled", buyError == BuyError.NoAccess );

		// Decrease the opacity to show that the item cannot be purchased
		// ex. lack of credits
		SetClass( "cannot-purchase", buyError != BuyError.None );
	}
}
