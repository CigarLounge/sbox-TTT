using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public static class TeamExtensions
{
	private static readonly Dictionary<Team, HashSet<Client>> _clients = new();
	private static readonly Dictionary<Team, ColorGroup> _properties = new();

	static TeamExtensions()
	{
		// Default teams.
		Team.None.SetProperties( "Nones", Color.Transparent );
		Team.Innocents.SetProperties( "Innocents", Color.FromBytes( 26, 196, 77 ) );
		Team.Traitors.SetProperties( "Traitors", Color.FromBytes( 223, 40, 52 ) );

		_clients.Add( Team.None, new HashSet<Client>() );
		_clients.Add( Team.Innocents, new HashSet<Client>() );
		_clients.Add( Team.Traitors, new HashSet<Client>() );
	}

	public static string GetTitle( this Team team )
	{
		return _properties[team].Title;
	}

	public static Color GetColor( this Team team )
	{
		return _properties[team].Color;
	}

	public static void SetProperties( this Team team, string title, Color color )
	{
		_properties[team] = new ColorGroup
		{
			Title = title,
			Color = color
		};
	}

	public static int GetCount( this Team team )
	{
		return _clients[team].Count;
	}

	public static To ToClients( this Team team )
	{
		return To.Multiple( _clients[team] );
	}

	public static To ToAliveClients( this Team team )
	{
		return To.Multiple( _clients[team].Where( x => x.Pawn.IsAlive() ) );
	}

	[GameEvent.Player.RoleChanged]
	private static void OnPlayerRoleChanged( Player player, Role oldRole )
	{
		if ( oldRole is not null )
			_clients[oldRole.Team].Remove( player.Client );

		if ( player.Role is not null )
			_clients[player.Team].Add( player.Client );
	}
}
