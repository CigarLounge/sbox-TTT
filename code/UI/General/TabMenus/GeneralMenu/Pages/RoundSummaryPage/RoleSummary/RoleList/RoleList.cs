using System;
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

	public RoleList( BaseRole role, long[] playerIds )
	{
		Header.Style.BackgroundColor = role.Color;
		Role.Text = $"{role.Title} - {playerIds.Length}";

		foreach ( var player in playerIds )
			AddPlayer( player );
	}

	private void AddPlayer( long playerId )
	{
		if ( !Game.Current.SavedClients.ContainsKey( playerId ) )
			return;

		var savedClient = Game.Current.SavedClients[playerId];

		var playerContainer = PlayersContainer.Add.Panel( "player" );
		playerContainer.Add.Image( $"avatar:{playerId}", "avatar" );
		playerContainer.Add.Label( savedClient.Name, "name-label" );
		playerContainer.Add.Panel( "spacer" );
		playerContainer.Add.Label( Math.Round( savedClient.BaseKarma - savedClient.ActiveKarma ).ToString(), "karma-label" );
		playerContainer.Add.Label( savedClient.RoundScore.ToString(), "score-label" );
	}
}
