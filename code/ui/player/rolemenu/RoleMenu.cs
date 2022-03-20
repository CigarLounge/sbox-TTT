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

		TabContainer.AddTab( new Shop(), "Shop", "shopping_cart" );
	}

	public void AddRadioTab()
	{
		TabContainer.AddTab( new RadioMenu(), "Radio", "radio" );
	}

	public void DeleteRadioTab()
	{
		TabContainer.DeleteTab( "Radio" );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( player.Role.Info.AvailableItems.Count == 0 || !player.IsAlive() )
		{
			SetClass( "fade-in", false );
			return;
		}

		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !HasClass( "fade-in" ) )
			return;

		RoleHeader.Text = player.Role.Info.Title;
		RoleHeader.Style.FontColor = player.Role.Info.Color;
	}
}
