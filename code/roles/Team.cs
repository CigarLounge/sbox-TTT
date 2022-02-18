using Sandbox;
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

	public static To ToClients( this Team team )
	{
		return To.Multiple( Client.All.Where( x => (x.Pawn as TTTPlayer).Role.Team == team ) );
	}
}

