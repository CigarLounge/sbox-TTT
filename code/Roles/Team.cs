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
	public static string GetTitle( this Team team )
	{
		return team switch
		{
			Team.Innocents => "Innocents",
			Team.Traitors => "Traitors",
			_ => "None",
		};
	}

	public static Color GetColor( this Team team )
	{
		return team switch
		{
			Team.Innocents => Color.FromBytes( 27, 197, 78 ),
			Team.Traitors => Color.FromBytes( 223, 41, 53 ),
			_ => Color.Transparent,
		};
	}

	// TODO: Koley ;), cache this. Maybe in a dictionary at run time?
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
}

