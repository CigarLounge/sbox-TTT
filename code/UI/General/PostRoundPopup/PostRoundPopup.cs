using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PostRoundPopup : Panel
{
	private Label Header { get; init; }
	private InputGlyph Glyph { get; init; }

	public PostRoundPopup() { }

	[GameEvent.Round.Ended]
	private static void DisplayWinner( Team winningTeam, WinType winType )
	{
		if ( !Host.IsClient )
			return;

		var roundPopup = new PostRoundPopup();
		Local.Hud.AddChild( roundPopup );

		roundPopup.Header.Text = winningTeam == Team.None ? "IT'S A TIE!" : $"THE {winningTeam.GetTitle()} WIN!";
		roundPopup.Header.Style.FontColor = winningTeam.GetColor();
	}

	public override void Tick()
	{
		Glyph.Style.Opacity = Input.Down( InputButton.View ) ? 1.0f : 0.5f;
	}

	[GameEvent.Round.Started]
	private void Close()
	{
		Delete();
	}
}
