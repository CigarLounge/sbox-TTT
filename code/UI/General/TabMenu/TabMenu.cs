using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TabMenu : Panel
{
	public static TabMenu Instance;

	private readonly Scoreboard _scoreboard;
	private readonly SettingsMenu _settingsMenu;
	private bool _isViewingSummaryPage = false;

	public TabMenu()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/TabMenu/TabMenu.scss" );

		var scoreboardButton = new Button( "Menu", "dehaze", SwapToMenu );
		scoreboardButton.AddClass( "scoreboard-button" );

		_scoreboard = new Scoreboard( this, scoreboardButton );
		_scoreboard.AddClass( "scoreboard" );

		var settingsMenuButton = new Button( "Scoreboard", "people", SwapToScoreboard );
		settingsMenuButton.AddClass( "settings-button" );

		_settingsMenu = new SettingsMenu( this, settingsMenuButton );
		_settingsMenu.AddClass( "settings" );
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	private void SwapToMenu()
	{
		_scoreboard.EnableFade( false );
		_settingsMenu.EnableFade( true );
	}

	private void SwapToScoreboard()
	{
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	[Event.BuildInput]
	private void MenuInput( InputBuilder input )
	{
		var isScoreDown = input.Down( InputButton.Score );

		SetClass( "show", isScoreDown );

		if ( Game.Current.Round is InProgressRound )
			return;

		if ( input.Released( InputButton.View ) )
			_isViewingSummaryPage = false;

		var isViewDown = input.Down( InputButton.View );
		if ( isViewDown && !_isViewingSummaryPage )
		{
			SwapToMenu();
			SettingsMenu.Instance.GoToPage( new RoundSummaryPage() );
			_isViewingSummaryPage = true;
		}

		SetClass( "show", isViewDown || isScoreDown );
	}
}
