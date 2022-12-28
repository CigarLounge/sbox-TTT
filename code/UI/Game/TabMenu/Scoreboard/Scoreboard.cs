using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	public Panel Buttons { get; set; }
	private Panel GroupPanel { get; set; }

	private PriorityQueue<Player, PlayerStatus> PlayersByStatus()
	{
		var players = new PriorityQueue<Player, PlayerStatus>();
		foreach ( var client in Game.Clients )
			if ( client.Pawn is Player player )
				players.Enqueue( player, player.Status );
		return players;
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		// AddChild( Buttons );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Game.Clients.HashCombine( client => HashCode.Combine( client.SteamId, (int)(client.Pawn as Player)?.Status ) ) );
	}
}
