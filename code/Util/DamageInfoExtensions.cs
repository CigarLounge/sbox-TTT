using Sandbox;

namespace TTT;

public static class DamageInfoExtensions
{
	/// <summary>
	/// Is this damage a headshot?
	/// </summary>
	public static bool IsHeadshot( this DamageInfo info ) => info.Hitbox.HasTag( "head" );
	/// <summary>
	/// If this damage ends up being lethal, should the player scream when killed?
	/// </summary>
	public static bool IsSilent( this DamageInfo info ) => info.HasTag( "silent" );
	/// <summary>
	/// If this damage was avoidable e.g. a traitor dying to a
	/// teammate's C4, then no karma penalty will be given to the teammate.
	/// </summary>
	public static bool IsAvoidable( this DamageInfo info ) => info.HasTag( "avoidable" );
}
