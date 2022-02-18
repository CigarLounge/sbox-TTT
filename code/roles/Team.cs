using Sandbox;
using System.Collections.Generic;
using System.Linq;

using TTT.Player;

namespace TTT.Roles;

public enum Team : byte
{
	None,
	Innocents,
	Traitors
}

public static class TeamExtensions
{
	public static string GetName( this Team team )
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
	public static IEnumerable<TTTPlayer> GetOthers( this Team team, TTTPlayer player )
	{
		return Entity.All.OfType<TTTPlayer>().Where( e => e.Team == team && player != e );
	}

	public static IEnumerable<TTTPlayer> GetAll( this Team team )
	{
		return Entity.All.OfType<TTTPlayer>().Where( e => e.Team == team );
	}

	public static int GetCount( this Team team )
	{
		return Entity.All.OfType<TTTPlayer>().Where( e => e.Team == team ).Count();
	}

	public static To ToClients( this Team team )
	{
		return To.Multiple( Client.All.Where( x => (x.Pawn as TTTPlayer).Role.Team == team ) );
	}
}

