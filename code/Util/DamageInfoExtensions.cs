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
	public static bool IsSilent( this DamageInfo info ) => info.HasTag( DamageTags.Silent );

	/// <summary>
	/// If this damage was avoidable e.g. a traitor dying to a
	/// teammate's C4, then no karma penalty will be given to the teammate.
	/// </summary>
	public static bool IsAvoidable( this DamageInfo info ) => info.HasTag( DamageTags.Avoidable );

	/// <summary>
	/// Creates a bloodsplatter effect in the world.
	/// </summary>
	/// <param name="info"></param>
	/// <param name="player">The attacked player, their blood is getting splattered.</param>
	/// <param name="maxDistance">How far we will check for a place to splatter.</param>
	public static void CreateBloodSplatter( this DamageInfo info, Player player, float maxDistance )
	{
		var splatterTrace = Trace.Ray( new Ray( info.Position, info.Force.Normal ), maxDistance )
			.Ignore( player )
			.Run();

		if ( !splatterTrace.Hit )
			return;

		var decal = ResourceLibrary.Get<DecalDefinition>( "decals/blood_splatter.decal" );
		Decal.Place( To.Everyone, decal, null, 0, splatterTrace.EndPosition - splatterTrace.Direction * 1f, Rotation.LookAt( splatterTrace.Normal ), Color.White );
	}
}

public static class DamageTags
{
	public const string Avoidable = "avoidable";
	public const string Silent = "silent";
	public const string Bullet = "bullet";
	public const string Drown = "drown";
	public const string Slash = "slash";
	public const string Burn = "burn";
	public const string Blast = "blast";
	public const string Vehicle = "vehicle";
	public const string Fall = "fall";
	public const string Explode = "explode";
	public const string IgnoreDamage = "ignoredamage";
}
