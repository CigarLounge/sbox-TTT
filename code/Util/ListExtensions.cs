using System.Collections.Generic;
using Sandbox;

namespace TTT;

public static class ListExtensions
{
	public static bool IsNullOrEmpty<T>( this IList<T> list ) => list is null || list.Count == 0;
	public static bool IsNullOrEmpty<T>( this T[] arr ) => arr is null || arr.Length == 0;

	public static void Shuffle<T>( this IList<T> list )
	{
		Rand.SetSeed( Time.Tick );
		var n = list.Count;
		while ( n > 1 )
		{
			n--;
			var k = Rand.Int( 0, n );
			(list[n], list[k]) = (list[k], list[n]);
		}
	}
}
