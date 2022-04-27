using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class TabMenu : Panel
{
	private Scoreboard _scoreboard;
	private SettingsMenu _settingsMenu;

	public TabMenu()
	{
		StyleSheet.Load( "/UI/General/TabMenu/TabMenu.scss" );

		var scoreboardButton = new Button( "Menu", "dehaze", () =>
		{
			_scoreboard.EnableFade( false );
			_settingsMenu.EnableFade( true );
		} );
		scoreboardButton.AddClass( "scoreboard-button" );

		_scoreboard = new Scoreboard( this, scoreboardButton );
		_scoreboard.AddClass( "scoreboard" );

		var settingsMenuButton = new Button( "Scoreboard", "people", () =>
		{
			_scoreboard.EnableFade( true );
			_settingsMenu.EnableFade( false );
		} );
		settingsMenuButton.AddClass( "settings-button" );

		_settingsMenu = new SettingsMenu( this, settingsMenuButton );
		_settingsMenu.AddClass( "settings" );
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	[Event.BuildInput]
	private void MenuInput( InputBuilder input )
	{
		SetClass( "show", input.Down( InputButton.Score ) );
	}
}
