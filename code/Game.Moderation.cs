using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public struct BannedClient
{
	public long SteamId { get; set; }
	public string Reason { get; set; }
	public DateTime Duration { get; set; }
}

public partial class Game : Sandbox.Game
{
	public static readonly List<BannedClient> BannedClients = new();
	public const string BanFilePath = "bans.json";

	public override bool ShouldConnect( long playerId )
	{
		if ( Karma.SavedPlayerValues.TryGetValue( playerId, out var value ) )
			if ( value < Karma.MinValue )
				return false;

		foreach ( var bannedClient in BannedClients )
		{
			if ( bannedClient.SteamId != playerId )
				continue;

			if ( bannedClient.Duration >= DateTime.Now )
				return false;

			BannedClients.Remove( bannedClient );
		}

		return true;
	}

	private void LoadBannedClients()
	{
		var clients = FileSystem.Data.ReadJson<List<BannedClient>>( BanFilePath );
		if ( !clients.IsNullOrEmpty() )
			BannedClients.AddRange( clients );
	}

	[ConCmd.Admin( Name = "ttt_ban", Help = "Ban the client with the following steam id." )]
	public static void BanPlayer( string rawSteamId, int minutes = default, string reason = "" )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var client in Client.All )
		{
			if ( client.PlayerId != steamId )
				continue;

			client.Ban( minutes, reason );
			return;
		}

		// Player isn't currently in the server, we should ban anyways.
		BannedClients.Add(
			new BannedClient
			{
				SteamId = steamId,
				Duration = minutes == default ? DateTime.MaxValue : DateTime.Now.AddMinutes( minutes ),
				Reason = reason
			}
		);
	}

	[ConCmd.Admin( Name = "ttt_ban_remove", Help = "Remove the ban on a client using a steam id." )]
	public static void RemoveBanWithSteamId( string rawSteamId )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var bannedClient in BannedClients )
		{
			if ( bannedClient.SteamId != steamId )
				continue;

			BannedClients.Remove( bannedClient );
			return;
		}

		Log.Warning( $"Unable to find player with steam id {rawSteamId}" );
	}

	[ConCmd.Admin( Name = "ttt_kick", Help = "Kick the client with the following steam id." )]
	public static void KickPlayer( string rawSteamId )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var client in Client.All )
		{
			if ( client.PlayerId == steamId )
				continue;

			client.Kick();
			return;
		}

		Log.Warning( $"Unable to find player with steam id {rawSteamId}" );
	}
}
