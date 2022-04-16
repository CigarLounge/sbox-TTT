using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardHeader : Panel
{
	private Label PlayerCount { get; set; }
	private Label CurrentMap { get; set; }
	private Label MapChange { get; set; }

	public ScoreboardHeader()
	{
		CurrentMap.Text = Global.MapName;
	}

	public override void Tick()
	{
		PlayerCount.Text = $"{Client.All.Count} / {ConsoleSystem.GetValue( "maxplayers" ).ToInt( 0 )} Players";

		var roundsRemaining = Game.RoundLimit - Game.Current.TotalRoundsPlayed;
		var suffix = roundsRemaining == 1 ? "round" : "rounds";
		MapChange.Text = $"Map will change in {roundsRemaining} {suffix}";
	}
}
