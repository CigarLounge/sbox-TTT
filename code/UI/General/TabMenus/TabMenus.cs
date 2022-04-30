using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TabMenus : Panel
{
	public static TabMenus Instance;

	private readonly Scoreboard _scoreboard;
	private readonly GeneralMenu _settingsMenu;
	private bool _isViewingSummaryPage = false;

	public TabMenus()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/TabMenus/TabMenus.scss" );

		var scoreboardButton = new Button( "Menu", "dehaze", SwapToMenu );
		scoreboardButton.AddClass( "scoreboard-button" );

		_scoreboard = new Scoreboard( this, scoreboardButton );
		_scoreboard.AddClass( "scoreboard" );

		var settingsMenuButton = new Button( "Scoreboard", "people", SwapToScoreboard );
		settingsMenuButton.AddClass( "settings-button" );

		_settingsMenu = new GeneralMenu( this, settingsMenuButton );
		_settingsMenu.AddClass( "settings" );
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	public void SwapToMenu()
	{
		_scoreboard.EnableFade( false );
		_settingsMenu.EnableFade( true );
	}

	public void SwapToScoreboard()
	{
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	[Event.BuildInput]
	private void MenuInput( InputBuilder input )
	{
		var isScoreDown = input.Down( InputButton.Score );

		SetClass( "show", isScoreDown );

		if ( Game.Current.State is InProgress )
			return;

		if ( input.Released( InputButton.View ) )
			_isViewingSummaryPage = false;

		var isViewDown = input.Down( InputButton.View );
		if ( isViewDown && !_isViewingSummaryPage )
		{
			SwapToMenu();
			GeneralMenu.Instance.GoToPage( new RoundSummaryPage() );
			_isViewingSummaryPage = true;
		}

		SetClass( "show", isViewDown || isScoreDown );
	}
}
