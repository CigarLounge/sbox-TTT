using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class TabMenu : Panel
{
	private Scoreboard _scoreboard;
	private SettingsMenu _settingsMenu;

	public TabMenu()
	{
		StyleSheet.Load( "/ui/generalhud/tabmenu/TabMenu.scss" );

		_scoreboard = new Scoreboard( this, new Button( "Settings", "settings", () =>
		{
			_scoreboard.EnableFade( false );
			_settingsMenu.EnableFade( true );
		} ) );
		_scoreboard.AddClass( "scoreboard" );

		_settingsMenu = new SettingsMenu( this, new Button( "Scoreboard", "people", () =>
		{
			_scoreboard.EnableFade( true );
			_settingsMenu.EnableFade( false );
		} ) );
		_settingsMenu.AddClass( "settings" );
	}

	public override void Tick()
	{

	}
}
