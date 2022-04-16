using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class PostRoundMenu : Panel
{
	public static PostRoundMenu Instance;

	private Label Header { get; init; }
	private Label Content { get; init; }

	private Team _winningTeam;

	public PostRoundMenu()
	{
		Instance = this;
	}

	[ClientRpc]
	public static void DisplayWinner( Team team )
	{
		Local.Hud.AddChild( new PostRoundMenu() );
		Instance._winningTeam = team;

		Instance.Open();
	}

	public void Close()
	{
		Delete();
		Instance = null;
	}

	public void Open()
	{
		Content.Text = "Thanks for playing TTT, more updates and stats to come!";

		Header.Text = _winningTeam == Team.None ? "IT'S A TIE!" : $"THE {_winningTeam.GetTitle()} WIN!";
		Header.Style.FontColor = _winningTeam.GetColor();
	}
}
