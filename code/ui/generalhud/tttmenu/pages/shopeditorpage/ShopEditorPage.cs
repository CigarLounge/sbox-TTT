using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Items;
using TTT.Roles;

namespace TTT.UI
{
	[UseTemplate]
	public partial class ShopEditorPage : Panel
	{
		public readonly List<QuickShopItem> ShopItems = new();

		private Panel Controls { get; set; }
		private Panel RoleShopContent { get; set; }
		private readonly Checkbox _checkbox;

		public ShopEditorPage()
		{
			Panel wrapper = new( Controls );

			DropDown roleDropdown = new DropDown( wrapper );
			roleDropdown.AddEventListener( "onchange", () =>
			 {
				 bool hasRoleSelected = roleDropdown.Selected != null && roleDropdown.Selected.Value is TTTRole role;
				 _checkbox.SetClass( "inactive", !hasRoleSelected );

				 if ( hasRoleSelected )
				 {
					 CreateRoleShopContent( roleDropdown.Selected.Value as TTTRole );
				 }
			 } );

			foreach ( Type roleType in Utils.GetTypes<TTTRole>() )
			{
				TTTRole role = Utils.GetObjectByType<TTTRole>( roleType );

				if ( role == null )
				{
					continue;
				}

				Option option = new( new string( role.Name ), role );
				roleDropdown.Options.Add( option );
				roleDropdown.Selected = option;
			}

			wrapper.Add.HorizontalLineBreak();

			_checkbox = new Checkbox();
			_checkbox.Parent = wrapper;
			_checkbox.AddClass( "inactive" );
			_checkbox.AddEventListener( "onchange", () =>
			 {
				 if ( roleDropdown.Selected.Value is TTTRole role )
				 {
					 ServerToggleShop( role.Name, _checkbox.Checked );
				 }
			 } );
		}

		private void CreateRoleShopContent( TTTRole selectedRole )
		{
			RoleShopContent.DeleteChildren( true );
			ShopItems.Clear();

			_checkbox.Checked = selectedRole.Shop.Enabled;

			foreach ( Type itemType in Utils.GetTypesWithAttribute<IItem, BuyableAttribute>() )
			{
				ShopItemData shopItemData = ShopItemData.CreateItemData( itemType );

				if ( shopItemData == null )
				{
					continue;
				}

				Panel wrapper = new( RoleShopContent );
				wrapper.AddClass( "row" );

				QuickShopItem item = new( wrapper );
				item.SetItem( shopItemData );
				item.AddEventListener( "onclick", () =>
				 {
					 ToggleItem( item, selectedRole );
				 } );

				foreach ( ShopItemData loopItemData in selectedRole.Shop.Items )
				{
					if ( loopItemData.Name.Equals( shopItemData.Name ) )
					{
						shopItemData.CopyFrom( loopItemData );

						item.SetItem( shopItemData );
						item.SetClass( "selected", true );
					}
				}

				wrapper.Add.HorizontalLineBreak();

				wrapper.Add.Button( "", "settings", () =>
				 {
					 EditItem( item, selectedRole );
				 } );

				wrapper.Add.LineBreak();

				ShopItems.Add( item );
			}
		}
	}
}
