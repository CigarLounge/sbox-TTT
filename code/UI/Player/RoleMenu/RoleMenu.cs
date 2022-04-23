using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleMenu : Panel
{
	public static RoleMenu Instance;

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
		TabContainer.AddTab( new Shop(), Strings.ShopTab, "shopping_cart" );
	}

	public void AddRadioTab()
	{
		TabContainer.AddTab( new RadioMenu(), Strings.RadioTab, "radio" );
	}

	public void AddDNATab()
	{
		TabContainer.AddTab( new DNAMenu(), Strings.DNATab, "fingerprint" );
	}

	public void RemoveTab( string tabName )
	{
		TabContainer.RemoveTab( tabName );
	}

	public override void Tick()
	{
		var player = Local.Pawn as Player;
		if ( !player.IsAlive() || TabContainer.Tabs.Count == 0 )
		{
			SetClass( "fade-in", false );
			return;
		}

		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !HasClass( "fade-in" ) )
			return;

		RoleHeader.Text = player.Role.Title;
		RoleHeader.Style.FontColor = player.Role.Color;
	}
}
