using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class RoleMenu : Panel
{
	public static RoleMenu Instance;

	public const string RadioTab = "Radio";
	public const string ShopTab = "Shop";
	public const string DNATab = "DNA";

	private CustomTabContainer TabContainer { get; set; }

	public RoleMenu() => Instance = this;

	public void AddShopTab() => TabContainer.AddTab( new Shop(), ShopTab, "shopping_cart", true );
	public void AddRadioTab() => TabContainer.AddTab( new RadioMenu(), RadioTab, "radio" );
	public void AddDNATab() => TabContainer.AddTab( new DNAMenu(), DNATab, "fingerprint" );
	public void RemoveTab( string tabName ) => TabContainer.RemoveTab( tabName );

	public override void Tick() => SetClass( "fade-in", Input.Down( InputButton.View ) && TabContainer.Tabs.Count > 0 );

	protected override int BuildHash() => HashCode.Combine( (Game.LocalPawn as Player).Role );
}
