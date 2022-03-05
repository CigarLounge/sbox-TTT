using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace TTT;

public static partial class Utils
{
	public static List<Client> GetClients( Func<Player, bool> predicate = null )
	{
		List<Client> clients = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player && (predicate == null || predicate.Invoke( player )) )
			{
				clients.Add( client );
			}
		}

		return clients;
	}

	public static List<Player> GetPlayers( Func<Player, bool> predicate = null )
	{
		List<Player> players = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player && (predicate == null || predicate.Invoke( player )) )
			{
				players.Add( player );
			}
		}

		return players;
	}

	public static List<Client> GetClientsWithRole( BaseRole role ) => GetClients( ( pl ) => pl.Role == role );
	public static List<Player> GetAlivePlayers() => GetPlayers( ( pl ) => pl.IsAlive() );
	public static List<Client> GetDeadClients() => GetClients( ( pl ) => !pl.IsAlive() );

	public static List<Client> GiveAliveDetectivesCredits( int credits )
	{
		List<Client> players = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player && player.IsAlive() && player.Role is DetectiveRole )
			{
				player.Credits += credits;
				players.Add( client );
			}
		}

		return players;
	}

	public static bool HasMinimumPlayers() => GetPlayers( ( pl ) => !pl.IsForcedSpectator ).Count >= Game.MinPlayers;

	public static int MinimumPlayerCount() => GetPlayers( ( pl ) => !pl.IsForcedSpectator ).Count;

	/// <summary>
	/// Returns an approximate value for meters given the Source engine units (for distances)
	/// based on https://developer.valvesoftware.com/wiki/Dimensions
	/// </summary>
	/// <param name="sourceUnits"></param>
	/// <returns>sourceUnits in meters</returns>
	public static float SourceUnitsToMeters( float sourceUnits ) => sourceUnits / 39.37f;

	/// <summary>
	/// Returns seconds in the format mm:ss
	/// </summary>
	/// <param name="seconds"></param>
	/// <returns>Seconds as a string in the format "mm:ss"</returns>
	public static string TimerString( float seconds ) => TimeSpan.FromSeconds( seconds.CeilToInt() ).ToString( @"mm\:ss" );

	public static void Enabled( this Panel panel, bool enabled )
	{
		panel.SetClass( "disabled", !enabled );
	}

	public static void EnableFade( this Panel panel, bool enabled )
	{
		panel.SetClass( "fade-in", enabled );
		panel.SetClass( "fade-out", !enabled );
	}

	public static bool IsEnabled( this Panel panel )
	{
		return !panel.HasClass( "disabled" );
	}

	public static void SetImage( this Image image, string imagePath )
	{
		image.Style.BackgroundImage = Texture.Load( FileSystem.Mounted, imagePath, false ) ?? Texture.Load( FileSystem.Mounted, $"/ui/none.png" );
	}

	public static bool HasGreatorOrEqualAxis( this Vector3 local, Vector3 other )
	{
		return local.x >= other.x || local.y >= other.y || local.z >= other.z;
	}

	/// <summary>
	/// Adds the item to the IList if that IList does not already contain the item
	/// </summary>
	public static void AddIfDoesNotContain<T>( this IList<T> list, T item )
	{
		if ( !list.Contains( item ) )
		{
			list.Add( item );
		}
	}

	public static void Shuffle<T>( this IList<T> list )
	{
		int n = list.Count;
		while ( n > 1 )
		{
			n--;
			int k = Rand.Int( 0, n );
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static bool IsNullOrEmpty<T>( this IList<T> list )
	{
		return list == null || list.Count == 0;
	}

	/// <summary>
	/// Checks if a C# array is null or empty
	/// </summary>
	public static bool IsNullOrEmpty<T>( this T[] arr )
	{
		return arr == null || arr.Length == 0;
	}

	public static bool IsAlive( this Entity entity ) => entity.LifeState == LifeState.Alive;
}
