using System;
using System.Collections.Generic;
using System.Linq;

using Sandbox;
using Sandbox.UI;
using TTT.Gamemode;
using TTT.Player;
using TTT.Roles;

namespace TTT.Globals;

public static partial class Utils
{
	public static List<Client> GetClients( Func<TTTPlayer, bool> predicate = null )
	{
		List<Client> clients = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is TTTPlayer player && (predicate == null || predicate.Invoke( player )) )
			{
				clients.Add( client );
			}
		}

		return clients;
	}

	public static List<TTTPlayer> GetPlayers( Func<TTTPlayer, bool> predicate = null )
	{
		List<TTTPlayer> players = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is TTTPlayer player && (predicate == null || predicate.Invoke( player )) )
			{
				players.Add( player );
			}
		}

		return players;
	}

	public static List<TTTPlayer> GetAlivePlayers() => GetPlayers( ( pl ) => pl.LifeState == LifeState.Alive );

	public static List<Client> GiveAliveDetectivesCredits( int credits )
	{
		List<Client> players = new();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is TTTPlayer player && player.LifeState == LifeState.Alive && player.Role is DetectiveRole )
			{
				player.Credits += credits;
				players.Add( client );
			}
		}

		return players;
	}

	public static bool HasMinimumPlayers() => GetPlayers( ( pl ) => !pl.IsForcedSpectator ).Count >= Gamemode.Game.MinPlayers;

	/// <summary>
	/// Loops through every type derived from the given type and collects non-abstract types.
	/// </summary>
	/// <returns>List of all available types of the given type</returns>
	public static List<Type> GetTypes<T>() => GetTypes<T>( null );

	/// <summary>
	/// Loops through every type derived from the given type and collects non-abstract types that matches the given predicate.
	/// </summary>
	/// <param name="predicate">a filter function to limit the scope</param>
	/// <returns>List of all available and matching types of the given type</returns>
	public static List<Type> GetTypes<T>( Func<Type, bool> predicate )
	{
		IEnumerable<Type> types = Library.GetAll<T>().Where( t => !t.IsAbstract && !t.ContainsGenericParameters );

		if ( predicate != null )
		{
			types = types.Where( predicate );
		}

		return types.ToList();
	}

	public static List<Type> GetTypesWithAttribute<T, U>( bool inherit = false ) where U : Attribute => GetTypes<T>( ( t ) => HasAttribute<U>( t, inherit ) );

	/// <summary>
	/// Get a derived `Type` of the given type by it's name (`Sandbox.LibraryAttribute`).
	/// </summary>
	/// <param name="name">The name of the `Sandbox.LibraryAttribute`</param>
	/// <returns>Derived `Type` of given type</returns>
	public static Type GetTypeByLibraryTitle<T>( string name )
	{
		foreach ( Type type in GetTypes<T>() )
		{
			if ( GetLibraryTitle( type ).Equals( name ) )
			{
				return type;
			}
		}

		return null;
	}

	/// <summary>
	/// Returns an instance of the given type by the given type `Type`.
	/// </summary>
	/// <param name="type">A derived `Type` of the given type</param>
	/// <returns>Instance of the given type object</returns>
	public static T GetObjectByType<T>( Type type ) => Library.Create<T>( type );

	/// <summary>
	/// Returns the `Sandbox.LibraryAttribute`'s `Name` of the given `Type`.
	/// </summary>
	/// <param name="type">A `Type` that has a `Sandbox.LibraryAttribute`</param>
	/// <returns>`Sandbox.LibraryAttribute`'s `Name`</returns>
	public static string GetLibraryName( Type type ) => Library.GetAttribute( type ).Name;

	/// <summary>
	/// Returns the `Sandbox.LibraryAttribute`'s `Title` of the given `Type`.
	/// </summary>
	/// <param name="type">A `Type` that has a `Sandbox.LibraryAttribute`</param>
	/// <returns>`Sandbox.LibraryAttribute`'s `Title`</returns>
	public static string GetLibraryTitle( Type type ) => Library.GetAttribute( type ).Title;

	public static T GetAttribute<T>( Type type ) where T : Attribute
	{
		foreach ( object obj in type.GetCustomAttributes( false ) )
		{
			if ( obj is T t )
			{
				return t;
			}
		}

		return default;
	}

	public static bool HasAttribute<T>( Type type, bool inherit = false ) where T : Attribute => type.IsDefined( typeof( T ), inherit );

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

	public static string GetTypeName( Type type ) => type.FullName.Replace( type.Namespace, "" ).TrimStart( '.' );

	public enum Realm
	{
		Client,
		Server
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

	/// <summary>
	/// Checks if a C# array is null or empty
	/// </summary>
	public static bool IsNullOrEmpty<T>( this T[] arr )
	{
		return arr == null || arr.Length == 0;
	}
}
