using System;
using System.Collections.Generic;
using Sandbox;

namespace TTT;

public static class ListExtensions
{
	public static bool IsNullOrEmpty<T>( this IList<T> list ) => list is null || list.Count == 0;
	public static bool IsNullOrEmpty<T>( this T[] arr ) => arr is null || arr.Length == 0;
	private static readonly Random _random = new();

	public static void Shuffle<T>( this IList<T> list )
	{
		var n = list.Count;
		while ( n > 1 )
		{
			n--;
			var k = _random.Next( 0, n + 1 );
			(list[n], list[k]) = (list[k], list[n]);
		}
	}

	// TODO: Move, just here for debugging purposes.
	public static int HashCombine<T>( this IEnumerable<T> e, Func<T, decimal> selector )
	{
		var result = 0;

		foreach ( var el in e )
			result = HashCode.Combine( result, selector.Invoke( el ) );

		return result;
	}
}
