using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class QuickShop : Panel
{
	private readonly Player _player;

	private readonly Panel _backgroundPanel;
	private readonly Panel _quickshopContainer;
	private readonly Label _creditLabel;
	private readonly Panel _itemPanel;
	private readonly Label _itemDescriptionLabel;
	private static ItemInfo _selectedItemInfo;
	private readonly List<QuickShopItem> _items = new();

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
		if ( Local.Pawn is not Player player )
			return;

		_player = player;

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

		Init();
	}

	private void Init()
	{
		foreach ( var libraryName in _player.Role.Info.AvailableItems )
		{
			var weapon = Asset.GetInfo<ItemInfo>( libraryName );
			AddItem( weapon );
		}
	}

	private void AddItem( ItemInfo itemInfo )
	{
		QuickShopItem item = new( _itemPanel );
		item.InitItem( itemInfo );

		item.AddEventListener( "onmouseover", () =>
		{
			_selectedItemInfo = itemInfo;

			Update();
		} );

		item.AddEventListener( "onmouseout", () =>
		{
			_selectedItemInfo = null;

			Update();
		} );

		item.AddEventListener( "onclick", () =>
		{
			if ( item.IsDisabled )
			{
				return;
			}

			// TODO: DO PURCHASE HERE.

			Update();
		} );

		_items.Add( item );
	}

	public void Update()
	{
		_creditLabel.Text = $"You have ${_player.Credits}";

		foreach ( QuickShopItem item in _items )
		{
			item.Update();
		}

		_itemDescriptionLabel.SetClass( "fade-in", _selectedItemInfo != null );

		if ( _selectedItemInfo != null )
		{
			_itemDescriptionLabel.Text = $"The description for the {_selectedItemInfo?.Name ?? ""} will go here.";
		}
	}
}
