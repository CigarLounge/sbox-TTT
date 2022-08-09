using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PostRoundPopup : Panel
{
	public static PostRoundPopup Instance;
	private Label Header { get; init; }

	public PostRoundPopup() => Instance = this;

	[GameEvent.Round.Ended]
	private static void DisplayWinner( Team winningTeam, WinType winType )
	{
		if ( !Host.IsClient )
			return;

		Local.Hud.AddChild( new PostRoundPopup() );

		Instance.Header.Text = winningTeam == Team.None ? "IT'S A TIE!" : $"THE {winningTeam.GetTitle()} WIN!";
		Instance.Header.Style.FontColor = winningTeam.GetColor();
	}

	[GameEvent.Round.Started]
	private void Close()
	{
		Delete();
		Instance = null;
	}
}
