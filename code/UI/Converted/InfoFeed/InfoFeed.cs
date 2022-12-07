using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class InfoFeed : Panel
{
	public static InfoFeed Instance { get; private set; }

	public InfoFeed() => Instance = this;

	[ClientRpc]
	public static void AddEntry( string message )
	{
		var entry = new InfoFeedEntry();
		entry.Add.Label( message );
		Instance.AddChild( entry );
	}

	[ClientRpc]
	public static void AddEntry( string message, Color color )
	{
		var entry = new InfoFeedEntry();
		entry.Add.Label( message ).Style.FontColor = color;
		Instance.AddChild( entry );
	}

	[ClientRpc]
	public static void AddEntry( Player player, string message )
	{
		var entry = new InfoFeedEntry();
		entry.Add.Label( player.IsLocalPawn ? "You" : player.SteamName, "entry" ).Style.FontColor = !player.IsRoleKnown ? Color.White : player.Role.Color;
		entry.Add.Label( message );
		Instance.AddChild( entry );
	}

	[ClientRpc]
	public static void AddRoleEntry( RoleInfo roleInfo, string message )
	{
		var entry = new InfoFeedEntry();
		entry.Add.Label( $"{roleInfo.Title}s", "entry" ).Style.FontColor = roleInfo.Color;
		entry.Add.Label( message );
		Instance.AddChild( entry );
	}

	[ClientRpc]
	public static void AddPlayerToPlayerEntry( Player left, Player right, string message, string suffix = "" )
	{
		var entry = new InfoFeedEntry();
		entry.Add.Label( left.IsLocalPawn ? "You" : left.SteamName, "entry" ).Style.FontColor = !left.IsRoleKnown ? Color.White : left.Role.Color;
		entry.Add.Label( message );
		entry.Add.Label( right.IsLocalPawn ? "You" : right.SteamName, "entry" ).Style.FontColor = !right.IsRoleKnown ? Color.White : right.Role.Color;

		if ( !suffix.IsNullOrEmpty() )
			entry.Add.Label( suffix );

		Instance.AddChild( entry );
	}

	[GameEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player player )
	{
		AddPlayerToPlayerEntry
		(
			player.Corpse.Finder,
			player,
			"found the body of",
			$"({player.Role.Title})"
		);
	}

	[GameEvent.Round.Start]
	private void OnRoundStart()
	{
		this.Enabled( true );

		if ( Game.Current.State.HasStarted )
			return;

		if ( Local.Pawn is not Player player )
			return;

		// TODO: Revert.
		// if ( !TabMenus.Instance.IsVisible )
		// 	TabMenus.Instance.SwapToScoreboard();

		AddEntry( "Roles have been assigned and the round has begun..." );
		AddEntry( $"Traitors will receive an additional {Game.InProgressSecondsPerDeath} seconds per death." );

		var karma = MathF.Round( player.BaseKarma );
		var df = MathF.Round( 100f - player.DamageFactor * 100f );
		var damageFactor = df == 0 ? $"Your karma is {karma}, you'll deal full damage this round." : $"Your karma is {karma}, you'll deal {df}% reduced damage this round.";
		AddEntry( damageFactor );
	}

	[GameEvent.Round.End]
	private void OnRoundEnd( Team _, WinType _1 )
	{
		this.Enabled( false );
		DeleteChildren();
	}
}
