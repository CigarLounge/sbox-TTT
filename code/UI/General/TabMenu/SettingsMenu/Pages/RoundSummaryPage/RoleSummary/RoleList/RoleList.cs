using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class RoleList : Panel
{
	private Label RoleHeader { get; init; }
	private Panel PlayersContainer { get; init; }

	public RoleList( BaseRole role, Player[] players )
	{
		RoleHeader.Text = $"{role.Title} - {players.Length}";
		RoleHeader.Style.BackgroundColor = role.Color;

		foreach ( var player in players )
			AddPlayer( player );
	}

	private void AddPlayer( Player player )
	{
		var playerContainer = PlayersContainer.Add.Panel( "player" );
		playerContainer.Add.Image( $"avatar:{player.Client.PlayerId}", "avatar" );
		playerContainer.Add.Label( player.Client.Name, "name-label" );
		playerContainer.Add.Panel( "spacer" );
		playerContainer.Add.Label( player.Score.ToString(), "score-label" );
	}
}
