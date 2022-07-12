using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class RoleList : Panel
{
	private Panel Header { get; init; }
	private Label Role { get; init; }
	private Panel PlayersContainer { get; init; }

	public RoleList( Role role, List<Player> players )
	{
		Header.Style.BackgroundColor = role.Color;
		Role.Text = $"{role.Title} - {players.Count}";

		foreach ( var player in players )
			AddPlayer( player );
	}

	// TODO: We need to handle null clients at some point.
	private void AddPlayer( Player player )
	{
		if ( !player.IsValid() || player.Client is null )
			return;

		var playerContainer = PlayersContainer.Add.Panel( "player" );
		playerContainer.Add.Image( $"avatar:{player.Client.PlayerId}", "avatar" );
		playerContainer.Add.Label( player.Client.Name, "name-label" );
		playerContainer.Add.Panel( "spacer" );
		playerContainer.Add.Label( Math.Round( player.BaseKarma ).ToString(), "karma-label" );
		playerContainer.Add.Label( player.Score.ToString(), "score-label" );
	}
}
