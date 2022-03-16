using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.Html;
using Sandbox.UI;

namespace TTT.UI;

/// <summary>
/// A container with tabs, allowing you to switch between different sheets.
/// 
/// You can position the tabs by adding the class tabs-bottom, tabs-left, tabs-right (default is tabs top)
/// </summary>
[Library( "tabcontainer", Alias = new[] { "tabcontrol", "tabs" } )]
public class TabContainer : Panel
{
	/// <summary>
	/// A control housing the tabs
	/// </summary>
	public Panel TabsContainer { get; protected set; }

	/// <summary>
	/// A control housing the sheets
	/// </summary>
	public Panel SheetContainer { get; protected set; }

	/// <summary>
	/// Access to the pages on this control
	/// </summary>
	public List<Tab> Tabs = new();

	/// <summary>
	/// If a cookie is set then the selected tab will be saved and restored.
	/// </summary>
	public string TabCookie { get; set; }

	public TabContainer()
	{
		AddClass( "tabcontainer" );

		TabsContainer = Add.Panel( "tabs" );
		SheetContainer = Add.Panel( "sheets" );
	}

	public override void SetProperty( string name, string value )
	{
		if ( name == "cookie" )
		{
			TabCookie = value;
			return;
		}

		base.SetProperty( name, value );
	}

	/// <summary>
	/// Add a tab to the sheet
	/// </summary>
	public Tab AddTab( Panel panel, string title, string icon = null )
	{
		var index = Tabs.Count;

		var tab = new Tab( this, title, icon, panel );
		Tabs.Add( tab );

		var cookieIndex = string.IsNullOrWhiteSpace( TabCookie ) ? -1 : Cookie.Get( $"dropdown.{TabCookie}", -1 );

		panel.Parent = SheetContainer;

		if ( index == 0 || cookieIndex == index )
		{
			SwitchTab( tab, false );
		}
		else
		{
			tab.Active = false;
		}

		return tab;
	}

	public void DeleteTab( string title )
	{
		for ( int i = Tabs.Count - 1; i >= 0; --i )
		{
			var tab = Tabs[i];
			if ( tab.Title == title )
			{
				tab.Page.Delete( true );
				tab.Button.Delete( true );
				Tabs.RemoveAt( i );
			}
		}

		if ( !Tabs.IsNullOrEmpty() )
			SwitchTab( Tabs.First() );
	}

	public override void OnTemplateSlot( INode element, string slotName, Panel panel )
	{
		if ( slotName == "tab" )
		{
			AddTab( panel, element.GetAttribute( "tabtext", null ), element.GetAttribute( "tabicon", null ) );
			return;
		}

		base.OnTemplateSlot( element, slotName, panel );
	}

	/// <summary>
	/// Switch to a specific tab
	/// </summary>
	public void SwitchTab( Tab tab, bool setCookie = true )
	{
		foreach ( var page in Tabs )
		{
			page.Active = page == tab;
		}

		if ( setCookie && !string.IsNullOrEmpty( TabCookie ) )
		{
			Cookie.Set( $"dropdown.{TabCookie}", Tabs.IndexOf( tab ) );
		}
	}

	/// <summary>
	/// Holds a Tab button and a Page for each sheet on the TabControl
	/// </summary>
	public class Tab
	{
		private TabContainer Parent;
		public Button Button { get; protected set; }
		public Panel Page { get; protected set; }
		public string Title { get; protected set; }

		public Tab( TabContainer tabControl, string title, string icon, Panel panel )
		{
			Parent = tabControl;
			Page = panel;
			Title = title;

			Button = new Button( title, icon, () => Parent?.SwitchTab( this, true ) );
			Button.Parent = tabControl.TabsContainer;
		}

		bool active;

		/// <summary>
		/// Change appearance based on active status
		/// </summary>
		public bool Active
		{
			get => active;
			set
			{
				active = value;
				Button.SetClass( "active", value );
				Page.SetClass( "active", value );
			}
		}
	}
}
