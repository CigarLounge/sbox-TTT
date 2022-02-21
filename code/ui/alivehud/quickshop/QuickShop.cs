using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class QuickShop : Panel
{
	private readonly Panel _backgroundPanel;
	private readonly Panel _quickshopContainer;
	private readonly Label _creditLabel;
	private readonly Panel _itemPanel;
	private readonly Label _itemDescriptionLabel;
	private static ItemInfo _selectedItemInfo;
	private readonly Dictionary<string, QuickShopItem> _shopItems = new();

	public enum BuyError
	{
		None,
		InventoryBlocked,
		NotEnoughCredits,
		NotAvailable,
		LimitReached
	}

	public QuickShop() : base()
	{
		StyleSheet.Load( "/ui/alivehud/quickshop/QuickShop.scss" );

		AddClass( "text-shadow" );

		_backgroundPanel = new Panel( this );
		_backgroundPanel.AddClass( "background-color-secondary" );
		_backgroundPanel.AddClass( "opacity-medium" );
		_backgroundPanel.AddClass( "fullscreen" );

		_quickshopContainer = new Panel( this );
		_quickshopContainer.AddClass( "quickshop-container" );

		_creditLabel = _quickshopContainer.Add.Label();
		_creditLabel.AddClass( "credit-label" );

		_itemPanel = new Panel( _quickshopContainer );
		_itemPanel.AddClass( "item-panel" );

		_itemDescriptionLabel = _quickshopContainer.Add.Label();
		_itemDescriptionLabel.AddClass( "item-description-label" );
	}

	public void AddRoleShopItems( Player player )
	{
		_shopItems.Clear();
		foreach ( var libraryName in player.Role.Info.AvailableItems )
		{
			var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );
			if ( itemInfo == null )
				return;

			AddRoleShopItem( itemInfo );
		}
	}

	private void AddRoleShopItem( ItemInfo itemInfo )
	{
		QuickShopItem item = new( _itemPanel, itemInfo );

		item.AddEventListener( "onmouseover", () => { _selectedItemInfo = itemInfo; } );
		item.AddEventListener( "onmouseout", () => { _selectedItemInfo = null; } );

		item.AddEventListener( "onclick", () =>
		{
			if ( item.IsDisabled )
				return;

			Player.PurchaseItem( itemInfo.LibraryName );
		} );

		_shopItems.Add( itemInfo.LibraryName, item );
	}

	public override void Tick()
	{
		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !HasClass( "fade-in" ) )
			return;

		if ( Local.Pawn is not Player player || player.Role.Info.AvailableItems.Count == 0 )
			return;

		_creditLabel.Text = $"You have ${player.Credits}";
		_itemDescriptionLabel.SetClass( "fade-in", _selectedItemInfo != null );
		if ( _selectedItemInfo != null )
			_itemDescriptionLabel.Text = $"The description for the {_selectedItemInfo?.Name ?? ""} will go here.";

		if ( _shopItems.Count == 0 )
			AddRoleShopItems( player );

		foreach ( var libraryName in player.PurchasedShopItems )
		{
			if ( _shopItems.TryGetValue( libraryName, out QuickShopItem shopItem ) )
			{
				shopItem.UpdateAvailability( true );
			}
		}
	}
}
