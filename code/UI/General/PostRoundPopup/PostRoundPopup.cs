using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class PostRoundPopup : Panel
{
	public static PostRoundPopup Instance;

	private Label Header { get; init; }

	public PostRoundPopup()
	{
		Instance = this;
	}

	[ClientRpc]
	public static void DisplayWinner( Team team )
	{
		Local.Hud.AddChild( new PostRoundPopup() );
		Instance.Open( team );
	}

	public void Close()
	{
		Delete();
		Instance = null;
	}

	public void Open( Team team )
	{
		Header.Text = team == Team.None ? "IT'S A TIE!" : $"THE {team.GetTitle()} WIN!";
		Header.Style.FontColor = team.GetColor();
	}
}
