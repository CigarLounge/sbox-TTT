using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardHeader : Panel
{
	private Label PlayerCount { get; set; }
	private Label CurrentMap { get; set; }

	public ScoreboardHeader()
	{
		UpdateServerInfo();
	}

	public void UpdateServerInfo()
	{
		int maxPlayers = ConsoleSystem.GetValue( "maxplayers" ).ToInt( 0 );

		PlayerCount.Text = $"{Client.All.Count} / {maxPlayers} Players";
		CurrentMap.Text = Global.MapName;
	}
}
