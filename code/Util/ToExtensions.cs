using System.Collections.Generic;
using Sandbox;

namespace TTT;

public static class ToExtensions
{
	public static To To( this List<Client> self )
	{
		return Sandbox.To.Multiple( self );
	}

	public static To To( this IEnumerable<Client> self )
	{
		return Sandbox.To.Multiple( self );
	}
}
