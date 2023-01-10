using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class ScoreboardInfo : Panel
{
	protected override int BuildHash()
	{
		return HashCode.Combine( Game.Clients.Count, GameManager.RoundLimit, GameManager.Current.TotalRoundsPlayed, GameManager.Current.MapTimer.Relative );
	}
}
