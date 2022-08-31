using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public struct BannedClient
{
	public long SteamId { get; set; }
	public string Reason { get; set; }
}

public partial class Game : Sandbox.Game
{
	private const string BanFilePath = "bans.json";
	private static List<BannedClient> _bannedClients = new();

	public override bool ShouldConnect( long playerId )
	{
		return !_bannedClients.Any( ( bannedClient ) => bannedClient.SteamId == playerId );
	}

	private void LoadBannedClients()
	{
		var clients = FileSystem.Data.ReadJson<List<BannedClient>>( BanFilePath );
		if ( !clients.IsNullOrEmpty() )
			_bannedClients = clients;
	}

	[ConCmd.Admin( Name = "ttt_ban_name", Help = "Ban the client with the following name." )]
	public static void BanPlayerWithName( string name, string reason = "" )
	{
		foreach ( var client in Client.All )
		{
			if ( client.Name != name )
				continue;

			BanPlayer( client, reason, true );
			return;
		}

		Log.Warning( $"Unable to find player with name {name}" );
	}

	[ConCmd.Admin( Name = "ttt_ban_steamid", Help = "Ban the client with the following steam id." )]
	public static void BanPlayerWithSteamId( string rawSteamId, string reason = "" )
	{
		var steamId = long.Parse( rawSteamId );

		foreach ( var client in Client.All )
		{
			if ( client.PlayerId != steamId )
				continue;

			BanPlayer( client, reason, true );
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

	public static void BanPlayer( Client client, string reason, bool writeBan )
	{
		if ( _bannedClients.Any( ( bannedClient ) => bannedClient.SteamId == client.PlayerId ) )
		{
			Log.Warning( $"{client.Name} is already banned!" );
			return;
		}

		client.Kick();
		_bannedClients.Add( new BannedClient { SteamId = client.PlayerId, Reason = reason } );

		if ( writeBan )
			FileSystem.Data.WriteJson( BanFilePath, _bannedClients );
	}

	[ConCmd.Admin( Name = "ttt_kick_name", Help = "Kick the client with the following name." )]
	public static void KickPlayerWithName( string name )
	{
		foreach ( var client in Client.All )
		{
			if ( client.Name != name )
				continue;

			client.Kick();
			return;
		}

		Log.Warning( $"Unable to find player with name {name}" );
	}

	[ConCmd.Admin( Name = "ttt_kick_steamid", Help = "Kick the client with the following steam id." )]
	public static void KickPlayerWithSteamId( string rawSteamId )
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
