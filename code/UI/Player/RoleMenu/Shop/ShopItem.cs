using Sandbox.UI;
namespace TTT.UI;

[UseTemplate]
public partial class ShopItem : Panel
{
	public ItemInfo ItemInfo { get; private set; }
	public bool IsDisabled { get; private set; }

	private Image Icon { get; init; }
	private Label Title { get; init; }
	private Label Price { get; init; }

	public ShopItem( ItemInfo item )
	{
		ItemInfo = item;
		Icon.SetTexture( item.Icon );
		Title.Text = item.Title;
		Price.Text = $"{item.Price}";
	}

	public void UpdateAvailability( bool canPurchase )
	{
		SetClass( "cannot-purchase", !canPurchase );
		IsDisabled = !canPurchase;
	}
}
