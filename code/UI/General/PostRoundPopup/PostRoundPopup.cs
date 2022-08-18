using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PostRoundPopup : Panel
{
	public static PostRoundPopup Instance { get; private set; }
	private Label Header { get; init; }
	private Label Content { get; init; }

	public PostRoundPopup() => Instance = this;

	[GameEvent.Round.Ended]
	private static void DisplayWinner( Team winningTeam, WinType winType )
	{
		if ( !Host.IsClient )
			return;

		Local.Hud.AddChild( new PostRoundPopup() );

		Instance.Header.Text = winningTeam == Team.None ? "IT'S A TIE!" : $"THE {winningTeam.GetTitle()} WIN!";
		Instance.Header.Style.FontColor = winningTeam.GetColor();

		switch ( winType )
		{
			case WinType.TimeUp:
			{
				Instance.Content.Text = "The Traitors ran out of time and lost!";
				break;
			}
			case WinType.Elimination:
			{
				if ( winningTeam == Team.Innocents )
					Instance.Content.Text = "The lovable Innocents eliminated the Traitors.";
				else if ( winningTeam == Team.Traitors )
					Instance.Content.Text = "The dastardly Traitors eliminated the Innocents.";

				break;
			}
		}
	}

	[GameEvent.Round.Started]
	private void Close()
	{
		Delete();
		Instance = null;
	}
}
