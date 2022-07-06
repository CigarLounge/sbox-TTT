using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace TTT;

public static class Utils
{
	private static List<Client> GetClients( Func<Player, bool> predicate )
	{
		List<Client> clients = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player && predicate.Invoke( player ) )
			{
				clients.Add( client );
			}
		}

		return clients;
	}

	private static List<Player> GetPlayers( Func<Player, bool> predicate )
	{
		List<Player> players = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player && predicate.Invoke( player ) )
			{
				players.Add( player );
			}
		}

		return players;
	}

	public static List<Client> GetAliveClientsWithRole( BaseRole role ) => GetClients( ( pl ) => pl.IsAlive() && pl.Role == role );
	public static List<Client> GetAliveClientsWithTeam( Team team ) => GetClients( ( pl ) => pl.IsAlive() && pl.Team == team );
	public static List<Client> GetClientsWithRole( BaseRole role ) => GetClients( ( pl ) => pl.Role == role );
	public static List<Client> GetDeadClients() => GetClients( ( pl ) => !pl.IsAlive() );

	public static List<Player> GetAlivePlayers() => GetPlayers( ( pl ) => pl.IsAlive() );
	public static List<Player> GetAlivePlayersWithRole( BaseRole role ) => GetPlayers( ( pl ) => pl.IsAlive() && pl.Role == role );

	public static bool HasMinimumPlayers() => MinimumPlayerCount() >= Game.MinPlayers;

	public static int MinimumPlayerCount() => GetPlayers( ( pl ) => !pl.IsForcedSpectator ).Count;

	/// <summary>
	/// Returns an approximate value for meters given the Source engine units (for distances)
	/// based on https://developer.valvesoftware.com/wiki/Dimensions
	/// </summary>
	/// <param name="sourceUnits"></param>
	/// <returns>sourceUnits in meters</returns>
	public static float SourceUnitsToMeters( this float sourceUnits ) => sourceUnits / 39.37f;

	/// <summary>
	/// Returns seconds in the format mm:ss
	/// </summary>
	/// <param name="seconds"></param>
	/// <returns>Seconds as a string in the format "mm:ss"</returns>
	public static string TimerString( this float seconds ) => (int)seconds < 0 ? $"+{TimeSpan.FromSeconds( seconds.CeilToInt() ):mm\\:ss}" : TimeSpan.FromSeconds( seconds.CeilToInt() ).ToString( @"mm\:ss" );

	public static bool HasGreatorOrEqualAxis( this Vector3 local, Vector3 other )
	{
		return local.x >= other.x || local.y >= other.y || local.z >= other.z;
	}

	public static void Shuffle<T>( this IList<T> list )
	{
		Rand.SetSeed( Time.Tick );
		var n = list.Count;
		while ( n > 1 )
		{
			n--;
			var k = Rand.Int( 0, n );
			(list[n], list[k]) = (list[k], list[n]);
		}
	}

	public static bool IsNullOrEmpty<T>( this IList<T> list )
	{
		return list is null || list.Count == 0;
	}

	/// <summary>
	/// Checks if a C# array is null or empty
	/// </summary>
	public static bool IsNullOrEmpty<T>( this T[] arr )
	{
		return arr is null || arr.Length == 0;
	}
}
