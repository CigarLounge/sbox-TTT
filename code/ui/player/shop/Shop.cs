using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class Shop : Panel
{
	private readonly Panel _backgroundPanel;
	private readonly Panel _shopContainer;
	private readonly Label _creditLabel;
	private readonly Panel _itemPanel;
	private readonly Label _itemDescriptionLabel;
	private static ItemInfo _selectedItemInfo;
	private readonly List<ShopItem> _shopItems = new();

	public Shop() : base()
	{
		StyleSheet.Load( "/ui/player/shop/Shop.scss" );

		AddClass( "text-shadow" );

		_backgroundPanel = new Panel( this );
		_backgroundPanel.AddClass( "background-color-secondary" );
		_backgroundPanel.AddClass( "opacity-medium" );
		_backgroundPanel.AddClass( "fullscreen" );

		_shopContainer = new Panel( this );
		_shopContainer.AddClass( "shop-container" );

		_creditLabel = _shopContainer.Add.Label();
		_creditLabel.AddClass( "credit-label" );

		_itemPanel = new Panel( _shopContainer );
		_itemPanel.AddClass( "item-panel" );

		_itemDescriptionLabel = _shopContainer.Add.Label();
		_itemDescriptionLabel.AddClass( "item-description-label" );
	}

	public void AddRoleShopItems( Player player )
	{
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
		ShopItem item = new( _itemPanel, itemInfo );

		item.AddEventListener( "onmouseover", () => { _selectedItemInfo = itemInfo; } );
		item.AddEventListener( "onmouseout", () => { _selectedItemInfo = null; } );
		item.AddEventListener( "onclick", () =>
		{
			if ( item.IsDisabled )
				return;

			Player.PurchaseItem( itemInfo.LibraryName );
		} );

		_shopItems.Add( item );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( player.Role.Info.AvailableItems.Count == 0 )
			return;

		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !HasClass( "fade-in" ) )
			return;

		_creditLabel.Text = $"You have ${player.Credits}";
		_itemDescriptionLabel.SetClass( "fade-in", _selectedItemInfo != null );
		if ( _selectedItemInfo != null )
			_itemDescriptionLabel.Text = $"The description for the {_selectedItemInfo?.Name ?? ""} will go here.";

		if ( _shopItems.Count == 0 )
			AddRoleShopItems( player );

		foreach ( var shopItem in _shopItems )
		{
			shopItem.UpdateAvailability( player.CanPurchase( shopItem.ItemInfo ) );
		}
	}

	[TTTEvent.Player.Role.Changed]
	private void OnRoleChange()
	{
		_shopItems.Clear();
	}
}
