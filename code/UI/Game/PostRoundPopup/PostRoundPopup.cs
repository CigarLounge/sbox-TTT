using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PostRoundPopup : Panel
{
	public static PostRoundPopup Instance { get; private set; }
	public Team WinningTeam { get; private set; }
	public WinType WinType { get; private set; }

	public PostRoundPopup() => Instance = this;

	[TTTEvent.Round.End]
	private static void DisplayWinner( Team winningTeam, WinType winType )
	{
		if ( !Game.IsClient )
			return;

		Game.RootPanel.AddChild( new PostRoundPopup() { WinningTeam = winningTeam, WinType = winType } );
	}

	[GameEvent.Entity.PostCleanup]
	private void Close()
	{
		Delete();
		Instance = null;
	}
}
