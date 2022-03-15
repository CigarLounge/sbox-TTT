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

		AddClass( "text-shadow" );

		TabContainer.AddTab( new Shop(), "Shop", "shopping_cart" );

		// TODO: Remove
		AddRadioTab();
	}

	public void AddRadioTab()
	{
		TabContainer.AddTab( new RadioMenu(), "Radio", "radio" );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( player.Role.Info.AvailableItems.Count == 0 )
			return;

		SetClass( "fade-in", Input.Down( InputButton.View ) );

		if ( !HasClass( "fade-in" ) )
			return;
	}

	[TTTEvent.Player.Role.Changed]
	private void OnRoleChange( Player player )
	{
		RoleHeader.Text = player.Role.Info.Title;
		RoleHeader.Style.FontColor = player.Role.Info.Color;
	}
}
