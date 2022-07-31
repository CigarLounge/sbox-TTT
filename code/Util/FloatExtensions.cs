using System;
using Sandbox;

namespace TTT;

public static class FloatExtensions
{
	/// <summary>
	/// Returns an approximate value for meters given the Source engine units (for distances)
	/// based on https://developer.valvesoftware.com/wiki/Dimensions
	/// </summary>
	/// <param name="sourceUnits"></param>
	/// <returns>sourceUnits in meters</returns>
	public static float SourceUnitsToMeters( this float sourceUnits ) => sourceUnits / 39.37f;

	/// <summary>
	/// Returns seconds in the format mm:ss
	/// </summary>
	/// <param name="seconds"></param>
	/// <returns>Seconds as a string in the format "mm:ss"</returns>
	public static string TimerString( this float seconds )
	{
		return (int)seconds < 0 ? $"+{TimeSpan.FromSeconds( seconds.CeilToInt() ):mm\\:ss}" : TimeSpan.FromSeconds( seconds.CeilToInt() ).ToString( @"mm\:ss" );
	}
}
