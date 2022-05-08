using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public partial class InfoFeed : Panel
{
	public static InfoFeed Instance;

	public InfoFeed() => Instance = this;

	public void AddEntry( string method, Color? color = null )
	{
		var e = Instance.AddChild<InfoFeedEntry>();
		var label = e.AddLabel( method, "method" );
		label.Style.FontColor = color ?? Color.White;
	}

	public void AddEntry( Client client, string message )
	{
		var e = Instance.AddChild<InfoFeedEntry>();

		bool isLocal = client == Local.Client;
		var player = client.Pawn as Player;

		var leftLabel = e.AddLabel( isLocal ? "You" : client.Name, "left" );
		leftLabel.Style.FontColor = player.Role is NoneRole ? Color.White : player.Role.Color;

		e.AddLabel( message, "method" );
	}

	public void AddRoleEntry( RoleInfo roleInfo, string interaction )
	{
		var e = Instance.AddChild<InfoFeedEntry>();

		var leftLabel = e.AddLabel( $"{roleInfo.Title}s", "left" );
		leftLabel.Style.FontColor = roleInfo.Color;

		e.AddLabel( interaction, "method" );
	}

	public void AddClientToClientEntry( Client leftClient, string rightClientName, Color rightClientRoleColor, string method, string postfix = "" )
	{
		var e = Instance.AddChild<InfoFeedEntry>();

		bool isLeftLocal = leftClient == Local.Client;
		bool isRightLocal = rightClientName == Local.Client.Name;

		var leftPlayer = leftClient.Pawn as Player;

		var leftLabel = e.AddLabel( isLeftLocal ? "You" : leftClient.Name, "left" );
		leftLabel.Style.FontColor = leftPlayer.Role is NoneRole ? Color.White : leftPlayer.Role.Color;

		e.AddLabel( method, "method" );

		var rightLabel = e.AddLabel( isRightLocal ? "You" : rightClientName, "right" );
		rightLabel.Style.FontColor = rightClientRoleColor;

		if ( !string.IsNullOrEmpty( postfix ) )
			e.AddLabel( postfix, "append" );
	}

	[ClientRpc]
	public static void DisplayEntry( string message )
	{
		Instance?.AddEntry( message );
	}

	[ClientRpc]
	public static void DisplayEntry( string message, Color color )
	{
		Instance?.AddEntry( message, color );
	}

	[ClientRpc]
	public static void DisplayClientEntry( string message )
	{
		Instance?.AddEntry( Local.Client, message );
	}

	[ClientRpc]
	public static void DisplayRoleEntry( RoleInfo roleInfo, string message )
	{
		Instance?.AddRoleEntry( roleInfo, message );
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

	[TTTEvent.Player.CreditsFound]
	private void OnCreditsFound( Player player, int creditsFound )
	{
		AddEntry
		(
			player.Client,
			$"found {creditsFound} credits!"
		);
	}

	[TTTEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		if ( !TabMenus.Instance.IsVisible )
			TabMenus.Instance.SwapToScoreboard();

		Instance.AddEntry( "Roles have been assigned and the round has begun..." );
		Instance.AddEntry( $"Traitors will receive an additional {Game.InProgressSecondsPerDeath} seconds per death." );

		if ( Local.Pawn is not Player player )
			return;

		float karma = MathF.Round( player.BaseKarma );

		string text;
		if ( karma >= 1000 )
			text = $"Your karma is {karma}, you'll deal full damage this round.";
		else
			text = $"Your karma is {karma}, you'll deal reduced damage this round.";

		Instance.AddEntry( text );
	}
}
