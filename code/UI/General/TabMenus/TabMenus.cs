using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class TabMenus : Panel
{
	public static TabMenus Instance;

	private readonly Scoreboard _scoreboard;
	private readonly GeneralMenu _settingsMenu;
	private readonly Button _muteButton;

	public TabMenus()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/TabMenus/TabMenus.scss" );

		var scoreboardButtons = new Panel();
		scoreboardButtons.AddClass( "spacing" );
		scoreboardButtons.Add.ButtonWithIcon( "Menu", "menu_open", string.Empty, SwapToMenu );
		scoreboardButtons.Add.ButtonWithIcon( "Round Summary", "leaderboard", string.Empty, SwapToRoundSummary );
		_muteButton = scoreboardButtons.Add.ButtonWithIcon( "Mute Alive Players", "volume_up", string.Empty, Game.ToggleMute );
		_scoreboard = new Scoreboard( this, scoreboardButtons );

		var settingsButtons = new Panel();
		settingsButtons.Add.ButtonWithIcon( "Scoreboard", "people", string.Empty, SwapToScoreboard );
		_settingsMenu = new GeneralMenu( this, settingsButtons );

		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	public void SwapToMenu()
	{
		_scoreboard.EnableFade( false );
		_settingsMenu.EnableFade( true );
		GeneralMenu.Instance.PopToHomePage();
	}

	public void SwapToRoundSummary()
	{
		SwapToMenu();
		GeneralMenu.Instance.GoToPage( new RoundSummaryPage() );
	}

	public void SwapToScoreboard()
	{
		_scoreboard.EnableFade( true );
		_settingsMenu.EnableFade( false );
	}

	public override void Tick()
	{
		if ( Local.Client.Pawn is not Player player )
			return;

		switch ( player.PlayersMuted )
		{
			case PlayersMute.None:
				_muteButton.Text = "Mute Alive Players";
				_muteButton.Icon = "volume_up";
				break;
			case PlayersMute.AlivePlayers:
				_muteButton.Text = "Mute Spectators";
				_muteButton.Icon = "volume_off";
				break;
			case PlayersMute.Spectators:
				_muteButton.Text = "Mute All Players";
				_muteButton.Icon = "volume_off";
				break;
			case PlayersMute.All:
				_muteButton.Text = "Unmute Players";
				_muteButton.Icon = "volume_off";
				break;
		}
	}

	[Event.BuildInput]
	private void MenuInput( InputBuilder input )
	{
		SetClass( "show", input.Down( InputButton.Score ) );
	}
}
