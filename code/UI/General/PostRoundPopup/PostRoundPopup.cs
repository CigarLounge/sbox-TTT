using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PostRoundPopup : Panel
{
	public static PostRoundPopup Instance;

	private Label Header { get; init; }

	public PostRoundPopup() => Instance = this;

	private void Open( Team team )
	{
		Header.Text = team == Team.None ? "IT'S A TIE!" : $"THE {team.GetTitle()} WIN!";
		Header.Style.FontColor = team.GetColor();
	}

	[GameEvent.Round.Started]
	private void Close()
	{
		Delete();
		Instance = null;
	}

	[GameEvent.Round.Ended]
	private static void DisplayWinner( Team winningTeam, WinType winType )
	{
		if ( !Host.IsClient )
			return;

		Local.Hud.AddChild( new PostRoundPopup() );
		Instance.Open( winningTeam );
	}
}
