using System;
using System.Collections.Generic;
using Sandbox.UI;

namespace TTT.UI;

public partial class ScoreboardGroup : Panel
{
	public PlayerStatus Status { get; set; }
	public HashSet<Player> Players { get; set; } = new();
	protected override int BuildHash() => HashCode.Combine( Players.HashCombine( p => p.NetworkIdent ) );
}
