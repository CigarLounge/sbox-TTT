using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class RoleList : Panel
{
	public Role Role { get; set; }
	public List<Player> Players { get; set; }
	private Panel PlayersContainer { get; set; }

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		foreach ( var player in Players )
			AddPlayer( player );
	}

	// TODO: We need to handle null clients at some point.
	private void AddPlayer( Player player )
	{
		if ( !player.IsValid() || player.Client is null )
			return;

		var playerContainer = PlayersContainer.Add.Panel( "player" );
		playerContainer.Add.Image( $"avatar:{player.Client.SteamId}", "avatar" );
		playerContainer.Add.Label( player.Client.Name, "name-label" );
		playerContainer.Add.Panel( "spacer" );
		playerContainer.Add.Label( Math.Round( player.BaseKarma ).ToString(), "karma-label" );
		playerContainer.Add.Label( player.Score.ToString(), "score-label" );
	}
}
