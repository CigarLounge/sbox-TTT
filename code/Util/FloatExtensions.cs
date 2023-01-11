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
	/// Returns a float as in timer format
	/// </summary>
	/// <param name="seconds">The amount of seconds</param>
	/// <param name="displayHours">If we should show hours in the formatted timer</param>
	public static string TimerFormat( this float seconds, bool displayHours = false )
	{
		var timer = TimeSpan.FromSeconds( seconds.CeilToInt() ).ToString( displayHours ? @"hh\:mm\:ss" : @"mm\:ss" );
		return (int)seconds < 0 ? $"+{timer}" : timer;
	}
}
