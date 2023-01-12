using Sandbox;
using System;

namespace TTT;

public static class FloatExtensions
{
	/// <summary>
	/// Converts source units to meters (approximate) https://developer.valvesoftware.com/wiki/Dimensions
	/// </summary>
	/// <param name="sourceUnits"></param>
	/// <returns>source units coverted to meters</returns>
	public static float SourceUnitsToMeters( this float sourceUnits ) => sourceUnits / 39.37f;

	/// <summary>
	/// Formats a float into a timer string
	/// </summary>
	/// <param name="seconds">The amount of seconds</param>
	public static string TimerFormat( this float seconds )
	{
		var timer = TimeSpan.FromSeconds( seconds.CeilToInt() ).ToString( @"mm\:ss" );
		return (int)seconds < 0 ? $"+{timer}" : timer;
	}
}
