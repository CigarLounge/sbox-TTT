using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class ScoreboardGroup : Panel
{
	public PlayerStatus Status { get; set; }
	public int Players { get; set; } = 0;
	protected override int BuildHash() => HashCode.Combine( Players );
}
