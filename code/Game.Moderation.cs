using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public struct BannedClient
{
	public long SteamId { get; set; }
	public string Reason { get; set; }
	public DateTime Length { get; set; }
}

public partial class Game : Sandbox.Game
{
	private const string BanFilePath = "bans.json";
	private static List<BannedClient> _bannedClients = new();

	private void LoadBannedClients()
	{
		var clients = FileSystem.Data.ReadJson<List<BannedClient>>( BanFilePath );
		if ( !clients.IsNullOrEmpty() )
			_bannedClients = clients;
	}

	public override bool ShouldConnect( long playerId )
	{
		foreach ( var bannedClient in _bannedClients )
		{
			if ( bannedClient.SteamId != playerId )
				continue;

			if ( bannedClient.Length >= DateTime.Now )
				return false;

			_bannedClients.Remove( bannedClient );
			FileSystem.Data.WriteJson( BanFilePath, _bannedClients );
		}

		return true;
	}

	[ConCmd.Admin( Name = "ttt_ban", Help = "Ban the client with the following steam id." )]
	public static void BanPlayer( string rawSteamId, int minutes = default, string reason = "" )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var client in Client.All )
		{
			if ( client.PlayerId != steamId )
				continue;

			client.Kick();
			_bannedClients.Add(
				new BannedClient
				{
					SteamId = client.PlayerId,
					Length = minutes == default ? DateTime.MaxValue : DateTime.Now.AddMinutes( minutes ),
					Reason = reason
				}
			);
			FileSystem.Data.WriteJson( BanFilePath, _bannedClients );
			return;
		}

		Log.Warning( $"Unable to find player with steam id {rawSteamId}" );
	}

	[ConCmd.Admin( Name = "ttt_ban_remove", Help = "Remove the ban on a client using a steam id." )]
	public static void RemoveBanWithSteamId( string rawSteamId )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var bannedClient in _bannedClients )
		{
			if ( bannedClient.SteamId != steamId )
				continue;

			_bannedClients.Remove( bannedClient );
			FileSystem.Data.WriteJson( BanFilePath, _bannedClients );
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
			if ( client.PlayerId != steamId )
				continue;

			client.Kick();
			return;
		}

		Log.Warning( $"Unable to find player with steam id {rawSteamId}" );
	}
}
