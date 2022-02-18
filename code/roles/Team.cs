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
	public static Color GetColor( this Team team )
	{
		switch ( team )
		{
			case Team.Innocents:
				return Color.FromBytes( 27, 197, 78 );
			case Team.Traitors:
				return Color.FromBytes( 223, 41, 53 );
			default:
				return Color.White;
		}
	}

	public static string GetName( this Team team )
	{
		switch ( team )
		{
			case Team.Innocents:
				return "Innocents";
			case Team.Traitors:
				return "Traitors";
			default:
				return "None";
		}
	}

	public static To ToClients( this Team team )
	{
		return To.Multiple( Client.All.Where( x => (x.Pawn as TTTPlayer).Role.Team == team ) );
	}
}

