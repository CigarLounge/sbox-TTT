using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TTT;

public static class Utils
{
	public static List<Player> GetAlivePlayers() => GetPlayers( ( pl ) => pl.IsAlive() );
	public static int MinimumPlayerCount() => GetPlayers( ( pl ) => !pl.IsForcedSpectator ).Count;
	public static bool HasMinimumPlayers() => MinimumPlayerCount() >= GameManager.MinPlayers;

	public static List<IClient> GetAliveClientsWithRole<T>() where T : Role => GetClients( ( pl ) => pl.IsAlive() && pl.Role is T );
	public static List<IClient> GetDeadClients() => GetClients( ( pl ) => !pl.IsAlive() );

	private static List<Player> GetPlayers( Func<Player, bool> predicate )
	{
		List<Player> players = new();
		foreach ( var client in Game.Clients )
			if ( client.Pawn is Player player && predicate.Invoke( player ) )
				players.Add( player );

		return players;
	}

	private static List<IClient> GetClients( Func<Player, bool> predicate )
	{
		List<IClient> clients = new();
		foreach ( var client in Game.Clients )
			if ( client.Pawn is Player player && predicate.Invoke( player ) )
				clients.Add( client );

		return clients;
	}

	public static async void DelayAction( float seconds, Action callback )
	{
		await GameTask.DelaySeconds( seconds );
		callback?.Invoke();
	}

	public static byte[] Serialize<T>( this T data ) => JsonSerializer.SerializeToUtf8Bytes( data );
	public static T Deserialize<T>( this byte[] bytes ) => JsonSerializer.Deserialize<T>( bytes );
}
