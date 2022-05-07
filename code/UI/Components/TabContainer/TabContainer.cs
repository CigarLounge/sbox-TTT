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

	public TabContainer()
	{
		AddClass( "tabcontainer" );

		TabsContainer = Add.Panel( "tabs" );
		SheetContainer = Add.Panel( "sheets" );
	}

	/// <summary>
	/// Add a tab to the sheet
	/// </summary>
	public void AddTab( Panel panel, string title, string icon = null, bool hasSortPriority = false )
	{
		if ( Tabs.Any( ( t ) => t.Title == title ) )
			return;

		var index = Tabs.Count;

		var tab = new Tab( this, title, icon, panel );
		Tabs.Add( tab );

		if ( hasSortPriority )
			TabsContainer.SortChildren( ( t ) => t == tab.Button ? 0 : 1 );

		panel.Parent = SheetContainer;

		if ( index == 0 || hasSortPriority )
			SwitchTab( tab );
		else
			tab.Active = false;
	}

	public void RemoveTab( string title )
	{
		var isActive = false;
		for ( int i = Tabs.Count - 1; i >= 0; --i )
		{
			var tab = Tabs[i];

			if ( tab.Title == title )
			{
				isActive = tab.Active;
				tab.Page.Delete( true );
				tab.Button.Delete( true );
				Tabs.RemoveAt( i );
				break;
			}
		}

		if ( isActive && Tabs.Count > 0 )
			SwitchTab( Tabs[^1] );
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
	public void SwitchTab( Tab tab )
	{
		foreach ( var page in Tabs )
		{
			page.Active = page == tab;
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

			Button = new( title, icon, () => Parent?.SwitchTab( this ) )
			{
				Parent = tabControl.TabsContainer
			};
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
