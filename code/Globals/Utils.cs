using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace TTT;

public static class Utils
{
	public static List<Player> GetPlayersWhere( Func<Player, bool> predicate )
	{
		List<Player> players = new();
		foreach ( var client in Game.Clients )
			if ( client.Pawn is Player player && predicate.Invoke( player ) )
				players.Add( player );

		return players;
	}

	public static List<IClient> GetClientsWhere( Func<Player, bool> predicate )
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
