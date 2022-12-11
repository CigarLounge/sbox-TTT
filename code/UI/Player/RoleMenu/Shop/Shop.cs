using System.Linq;
using Sandbox;
using Sandbox.UI;
namespace TTT.UI;

[UseTemplate]
public partial class Shop : Panel
{
	private Label Credits { get; init; }
	private Panel Items { get; init; }
	private Label Description { get; init; }

	private ItemInfo _selectedItem;

	public override void Tick()
	{
		if ( Game.LocalPawn is not Player player )
			return;

		Credits.Text = $" {player.Credits} credits";

		Description.SetClass( "fade-in", _selectedItem is not null );
		if ( _selectedItem is not null )
			Description.Text = _selectedItem.Description;

		foreach ( var shopItem in Items.Children.Cast<ShopItem>() )
			shopItem.UpdateAvailability( player.CanPurchase( shopItem.ItemInfo ) );
	}

	private void AddShopItem( ItemInfo itemInfo )
	{
		var shopItem = new ShopItem( itemInfo );

		shopItem.AddEventListener( "onmouseover", () => { _selectedItem = itemInfo; } );
		shopItem.AddEventListener( "onmouseout", () => { _selectedItem = null; } );
		shopItem.AddEventListener( "onclick", () =>
		{
			if ( shopItem.IsDisabled )
				return;

			Player.PurchaseItem( itemInfo.ResourceId );
		} );

		Items.AddChild( shopItem );
	}

	[GameEvent.Player.RoleChanged]
	private void OnRoleChange( Player player, Role oldRole )
	{
		if ( !player.IsLocalPawn )
			return;

		Items.DeleteChildren( true );
		foreach ( var item in player.Role.ShopItems )
			AddShopItem( item );
	}
}
