using Sandbox;
using Sandbox.UI;
using System;

namespace TTT.UI;

[UseTemplate]
public partial class InfoFeed : Panel
{
	public static InfoFeed Instance { get; private set; }

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
	public static void AddEntry( Player player, string message )
	{
		var entry = Instance.AddChild<InfoFeedEntry>();

		var leftLabel = entry.AddLabel( player.IsLocalPawn ? "You" : player.SteamName, "left" );
		leftLabel.Style.FontColor = !player.IsRoleKnown ? Color.White : player.Role.Color;

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
	public static void AddPlayerToPlayerEntry( Player left, Player right, string method, string suffix = "" )
	{
		var entry = Instance.AddChild<InfoFeedEntry>();

		var leftLabel = entry.AddLabel( left.IsLocalPawn ? "You" : left.SteamName, "left" );
		leftLabel.Style.FontColor = !left.IsRoleKnown ? Color.White : left.Role.Color;

		entry.AddLabel( method, "method" );

		var rightLabel = entry.AddLabel( right.IsLocalPawn ? "You" : right.SteamName, "right" );
		rightLabel.Style.FontColor = !right.IsRoleKnown ? Color.White : right.Role.Color;

		if ( !suffix.IsNullOrEmpty() )
			entry.AddLabel( suffix, "append" );
	}

	[GameEvent.Player.CorpseFound]
	private void OnCorpseFound( Player player )
	{
		AddPlayerToPlayerEntry
		(
			player.Corpse.Finder,
			player,
			"found the body of",
			$"({player.Role.Title})"
		);
	}

	[GameEvent.Round.RolesAssigned]
	private void OnRolesAssigned()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( !TabMenus.Instance.IsVisible )
			TabMenus.Instance.SwapToScoreboard();

		if ( !player.Client.IsValid() )
			return;

		AddEntry( "Roles have been assigned and the round has begun..." );
		AddEntry( $"Traitors will receive an additional {Game.InProgressSecondsPerDeath} seconds per death." );

		var karma = MathF.Round( player.BaseKarma );
		var df = MathF.Round( 100f - player.DamageFactor * 100f );

		string text;
		if ( df == 0 )
			text = $"Your karma is {karma}, you'll deal full damage this round.";
		else
			text = $"Your karma is {karma}, you'll deal {df}% reduced damage this round.";

		AddEntry( text );
	}
}
