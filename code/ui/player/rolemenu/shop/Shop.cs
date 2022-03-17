using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTT.UI;

public partial class Shop : Panel
{
	private readonly Panel _backgroundPanel;
	private readonly Panel _shopContainer;
	private readonly Label _roleLabel;
	private readonly Label _creditLabel;
	private readonly Panel _itemPanel;
	private readonly Panel _scrollIcon;
	private readonly Label _itemDescriptionLabel;
	private static ItemInfo _selectedItemInfo;
	private readonly List<ShopItem> _shopItems = new();

	public Shop() : base()
	{
		StyleSheet.Load( "/ui/player/rolemenu/shop/Shop.scss" );

		_shopContainer = new Panel( this );
		_shopContainer.AddClass( "shop-container" );

		var creditPanel = _shopContainer.Add.Panel();
		creditPanel.Add.Label( "You have ", "credit-label" );
		_creditLabel = creditPanel.Add.Label();
		_creditLabel.AddClass( "credits" );

		_itemPanel = new Panel( _shopContainer );
		_itemPanel.AddClass( "item-panel" );

		_scrollIcon = _shopContainer.Add.Icon( "south", "scroll-icon" );
		_scrollIcon.EnableFade( false );

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

			Player.PurchaseItem( itemInfo.Id );
		} );

		_shopItems.Add( item );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( player.Role.Info.AvailableItems.Count == 0 )
			return;

		_creditLabel.Text = $"{player.Credits} credits";
		_itemDescriptionLabel.SetClass( "fade-in", _selectedItemInfo != null );
		if ( _selectedItemInfo != null )
			_itemDescriptionLabel.Text = _selectedItemInfo.Description;

		if ( _shopItems.Count == 0 )
			AddRoleShopItems( player );

		foreach ( var shopItem in _shopItems )
		{
			shopItem.UpdateAvailability( player.CanPurchase( shopItem.ItemInfo ) );
		}

		_scrollIcon.EnableFade( _itemPanel.ScrollOffset.y < 10 && _shopItems.Count > 8 );
	}

	[TTTEvent.Player.Role.Changed]
	private void OnRoleChange( Player player, BaseRole oldRole )
	{
		_shopItems.ForEach( ( item ) => item.Delete( true ) );
		_shopItems.Clear();
	}
}
