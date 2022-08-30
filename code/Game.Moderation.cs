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

	[ConCmd.Admin( Name = "ttt_ban_name", Help = "Ban the user with the name" )]
	public static void BanPlayerWithName( string name, string reason = "" )
	{
		foreach ( var client in Client.All )
		{
			if ( client.Name != name )
				continue;

			BanPlayer( client, reason, true );
			break;
		}
	}

	[ConCmd.Admin( Name = "ttt_ban_steamid", Help = "Ban the user from the server" )]
	public static void BanPlayerWithSteamID( long steamId, string reason = "" )
	{
		foreach ( var client in Client.All )
		{
			if ( client.PlayerId != steamId )
				continue;

			BanPlayer( client, reason, true );
			break;
		}
	}

	public static void BanPlayer( Client client, string reason, bool saveBan )
	{
		if ( _bannedClients.Any( ( bannedClient ) => bannedClient.SteamId == client.PlayerId ) )
		{
			Log.Error( $"{client.Name} is already banned!" );
			return;
		}

		client.Kick();
		_bannedClients.Add( new BannedClient { SteamId = client.PlayerId, Reason = reason } );

		if ( saveBan )
			FileSystem.Data.WriteJson( BanFilePath, _bannedClients );
	}

	[ConCmd.Admin( Name = "ttt_kick_name", Help = "Kick the user with the name" )]
	public static void KickPlayerWithName( string name )
	{
		foreach ( var client in Client.All )
		{
			if ( client.Name != name )
				continue;

			client.Kick();
			break;
		}
	}

	[ConCmd.Admin( Name = "ttt_kick_steamid", Help = "Ban the user from the server" )]
	public static void KickPlayerWithSteamID( long steamId )
	{
		foreach ( var client in Client.All )
		{
			if ( client.PlayerId != steamId )
				continue;

			client.Kick();
			break;
		}
	}
}
