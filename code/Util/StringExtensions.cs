using System;

namespace TTT;

public static class StringExtensions
{
	public static bool IsNullOrEmpty( this string s ) => s is null || s.Length == 0;

	public static string FirstCharToUpper( this string input )
	{
		return input switch
		{
			null => throw new ArgumentNullException( nameof( input ) ),
			"" => throw new ArgumentException( $"{nameof( input )} cannot be empty", nameof( input ) ),
			_ => string.Concat( input[0].ToString().ToUpper(), input.AsSpan( 1 ) )
		};
	}
}
