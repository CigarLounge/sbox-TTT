using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public struct BannedClient
{
	public long SteamId { get; set; }
	public string Reason { get; set; }
	public DateTime Length { get; set; }
}

public partial class Game : Sandbox.Game
{
	public static readonly List<BannedClient> BannedClients = new();
	public const string BanFilePath = "bans.json";

	public override bool ShouldConnect( long playerId )
	{
		if ( Karma.SavedPlayerValues.TryGetValue( playerId, out var value ) )
			return value >= Karma.MinValue;

		foreach ( var bannedClient in BannedClients )
		{
			if ( bannedClient.SteamId != playerId )
				continue;

			if ( bannedClient.Length >= DateTime.Now )
				return false;

			BannedClients.Remove( bannedClient );
			FileSystem.Data.WriteJson( BanFilePath, BannedClients );
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

		Log.Warning( $"Unable to find player with steam id {rawSteamId}" );
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
			FileSystem.Data.WriteJson( BanFilePath, BannedClients );
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
