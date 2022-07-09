using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public partial class InfoFeed : Panel
{
	public static InfoFeed Instance;

	public InfoFeed() => Instance = this;

	[ClientRpc]
	public static void AddEntry( string method )
	{
		Instance.AddChild<InfoFeedEntry>().AddLabel( method, "method" );
	}

	[ClientRpc]
	public static void AddEntry( string method, Color color )
	{
		Instance.AddChild<InfoFeedEntry>().AddLabel( method, "method" ).Style.FontColor = color;
	}

	[ClientRpc]
	public static void AddEntry( Client client, string message )
	{
		var entry = Instance.AddChild<InfoFeedEntry>();

		var player = client.Pawn as Player;
		var leftLabel = entry.AddLabel( client == Local.Client ? "You" : client.Name, "left" );
		leftLabel.Style.FontColor = player.Role is NoneRole ? Color.White : player.Role.Color;

		entry.AddLabel( message, "method" );
	}

	[ClientRpc]
	public static void AddRoleEntry( RoleInfo roleInfo, string interaction )
	{
		var entry = Instance.AddChild<InfoFeedEntry>();

		var leftLabel = entry.AddLabel( $"{roleInfo.Title}s", "left" );
		leftLabel.Style.FontColor = roleInfo.Color;

		entry.AddLabel( interaction, "method" );
	}

	[ClientRpc]
	public static void AddClientToClientEntry( Client leftClient, string rightClientName, Color rightClientRoleColor, string method, string suffix = "" )
	{
		var entry = Instance.AddChild<InfoFeedEntry>();

		var leftPlayer = leftClient.Pawn as Player;
		var leftLabel = entry.AddLabel( leftClient == Local.Client ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

		entry.AddLabel( method, "method" );

		var rightLabel = entry.AddLabel( rightClientName == Local.Client.Name ? "You" : rightClientName, "right" );
		rightLabel.Style.FontColor = rightClientRoleColor;

		if ( !string.IsNullOrEmpty( suffix ) )
			entry.AddLabel( suffix, "append" );
	}

	[TTTEvent.Player.CorpseFound]
	private void OnCorpseFound( Player player )
	{
		AddClientToClientEntry
		(
			player.Confirmer.Client,
			player.Corpse.PlayerName,
			player.Role.Color,
			"found the body of",
			$"({player.Role.Title})"
		);
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		if ( !TabMenus.Instance.IsVisible )
			TabMenus.Instance.SwapToScoreboard();

		AddEntry( "Roles have been assigned and the round has begun..." );
		AddEntry( $"Traitors will receive an additional {Game.InProgressSecondsPerDeath} seconds per death." );

		if ( Local.Pawn is not Player player )
			return;

		var karma = MathF.Round( player.BaseKarma );
		var dF = MathF.Round( 100f - player.DamageFactor * 100f );

		string text;
		if ( dF == 0 )
			text = $"Your karma is {karma}, you'll deal full damage this round.";
		else
			text = $"Your karma is {karma}, you'll deal {dF}% reduced damage this round.";

		AddEntry( text );
	}
}
