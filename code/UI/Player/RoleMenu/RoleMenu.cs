using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleMenu : Panel
{
	public static RoleMenu Instance;

	public const string RadioTab = "Radio";
	public const string ShopTab = "Shop";
	public const string DNATab = "DNA";

	private Label RoleHeader { get; set; }
	private TabContainer TabContainer { get; set; }

	public RoleMenu()
	{
		Instance = this;

		var backgroundPanel = Add.Panel( "background" );
		backgroundPanel.AddClass( "background-color-secondary" );
		backgroundPanel.AddClass( "opacity-medium" );
		backgroundPanel.AddClass( "fullscreen" );
	}

	public void AddShopTab()
	{
		TabContainer.AddTab( new Shop(), ShopTab, "shopping_cart", true );
	}

	public void AddRadioTab()
	{
		TabContainer.AddTab( new RadioMenu(), RadioTab, "radio" );
	}

	public void AddDNATab()
	{
		TabContainer.AddTab( new DNAMenu(), DNATab, "fingerprint" );
	}

	public void RemoveTab( string tabName )
	{
		TabContainer.RemoveTab( tabName );
	}

	public override void Tick()
	{
		var player = Local.Pawn as Player;
		if ( !player.IsAlive() || TabContainer.Tabs.Count == 0 || Game.Current.State is not InProgress )
		{
			SetClass( "fade-in", false );
			return;
		}

		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !IsVisible )
			return;

		RoleHeader.Text = player.Role.Title;
		RoleHeader.Style.FontColor = player.Role.Color;
	}
}
