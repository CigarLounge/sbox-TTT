using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	public Panel Buttons { get; set; }
	private Panel GroupPanel { get; set; }

	private IEnumerable<Player> PlayersSortedByStatus()
	{
		var players = new List<Player>();
		foreach ( var client in Game.Clients )
			if ( client.Pawn is Player player )
				players.Add( player );

		return players.OrderBy( player => player.Status );
	}

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		AddChild( Buttons );
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( Game.Clients.HashCombine( client => HashCode.Combine( client.SteamId, (int)(client.Pawn as Player)?.Status ) ) );
	}
}
