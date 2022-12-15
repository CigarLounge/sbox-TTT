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
		CurrentMap.Text = Game.Server.MapIdent;
	}

	public override void Tick()
	{
		PlayerCount.Text = $"{Game.Clients.Count} / {ConsoleSystem.GetValue( "maxplayers" ).ToInt( 0 )} Players";

		var roundsRemaining = GameManager.RoundLimit - GameManager.Current.TotalRoundsPlayed;
		var suffix = roundsRemaining == 1 ? "round" : "rounds";
		MapChange.Text = $"Map will change in {roundsRemaining} {suffix}";
	}
}
