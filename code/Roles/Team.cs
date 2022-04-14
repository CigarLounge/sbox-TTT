using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public enum Team : byte
{
	None,
	Innocents,
	Traitors
}

public static class TeamExtensions
{
	private struct TeamProperties
	{
		public string Title { get; init; }
		public Color Color { get; init; }
	}

	static TeamExtensions()
	{
		// Default teams.

		_properties[Team.None] = new TeamProperties
		{
			Title = "Nones",
			Color = Color.Transparent
		};

		_properties[Team.Innocents] = new TeamProperties
		{
			Title = "Innocents",
			Color = Color.FromBytes( 27, 197, 78 )
		};

		_properties[Team.Traitors] = new TeamProperties
		{
			Title = "Traitors",
			Color = Color.FromBytes( 223, 41, 53 )
		};
	}

	private static readonly Dictionary<Team, TeamProperties> _properties = new();

	public static string GetTitle( this Team team )
	{
		return _properties[team].Title;
	}

	public static Color GetColor( this Team team )
	{
		return _properties[team].Color;
	}

	// TODO: Kole cache this. Maybe in a dictionary at run time?
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

