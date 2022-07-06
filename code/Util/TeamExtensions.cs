using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public static class TeamExtensions
{
	private static readonly Dictionary<Team, ColorGroup> _properties = new();

	static TeamExtensions()
	{
		// Default teams.
		Team.None.SetProperties( "Nones", Color.Transparent );
		Team.Innocents.SetProperties( "Innocents", Color.FromBytes( 26, 196, 77 ) );
		Team.Traitors.SetProperties( "Traitors", Color.FromBytes( 223, 40, 52 ) );
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

	public static IEnumerable<Player> GetOthers( this Team team, Player player )
	{
		return Entity.All.OfType<Player>().Where( e => e.Team == team && player != e );
	}

	public static IEnumerable<Player> GetAll( this Team team )
	{
		return Entity.All.OfType<Player>().Where( e => e.Team == team );
	}

	public static int GetCount( this Team team )
	{
		return Entity.All.OfType<Player>().Where( e => e.Team == team ).Count();
	}

	public static To ToClients( this Team team )
	{
		return To.Multiple( Client.All.Where( x => (x.Pawn as Player).Team == team ) );
	}

	public static To ToAliveClients( this Team team )
	{
		return To.Multiple( Client.All.Where( x => x.Pawn is Player player && player.Team == team && player.IsAlive() ) );
	}
}
