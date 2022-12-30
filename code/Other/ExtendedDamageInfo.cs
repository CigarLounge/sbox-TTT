using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

/// <summary>
/// Extension of <see cref="Sandbox.DamageInfo"/> to fit TTT's needs.
/// </summary>
public struct ExtendedDamageInfo
{
	/// <summary>
	/// The player that is attacking.
	/// </summary>
	public Entity Attacker { get; set; }
	/// <summary>
	/// The weapon that the attacker is using.
	/// </summary>
	public Carriable Weapon { get; set; }
	/// <summary>
	/// The position of the attacker at the time the damage was inflicted.
	/// </summary>
	public Vector3 Origin { get; set; }
	/// <summary>
	/// The position the damage is being inflicted (the bullet entry point).
	/// </summary>
	public Vector3 Position { get; set; }
	/// <summary>
	/// The force of the damage - for moving physics etc. This would be the trajectory
	/// of the bullet multiplied by the speed and mass.
	/// </summary>
	public Vector3 Force { get; set; }
	/// <summary>
	/// The actual amount of damage this attack causes.
	/// </summary>
	public float Damage { get; set; }
	/// <summary>
	/// Damage tags, extra information about this attack.
	/// </summary>
	public HashSet<string> Tags { get; set; }
	/// <summary>
	/// The physics body that was hit.
	/// </summary>
	public PhysicsBody Body { get; set; }
	/// <summary>
	/// The bone index that the hitbox was attached to.
	/// </summary>
	public int BoneIndex { get; set; }
	/// <summary>
	/// The hitbox (if any) that was hit.
	/// </summary>
	public Hitbox Hitbox { get; set; }
	/// <summary>
	/// Is this damage a headshot?
	/// </summary>
	public bool IsHeadshot => Hitbox.HasTag( "head" );
	/// <summary>
	/// If this damage ends up being lethal, should the player scream when killed?
	/// </summary>
	public bool IsSilent => HasTag( "silent" );
	/// <summary>
	/// If this damage was avoidable e.g. a traitor dying to a
	/// teammate's C4, then no karma penalty will be given to the teammate.
	/// </summary>
	public bool IsAvoidable => HasTag( "avoidable" );

	/// <summary>
	/// Creates a new DamageInfo with the "bullet" tag.
	/// </summary>
	/// <param name="hitPosition"></param>
	/// <param name="hitForce"></param>
	/// <param name="damage"></param>
	/// <returns></returns>
	public static ExtendedDamageInfo FromBullet( Vector3 hitPosition, Vector3 hitForce, float damage )
	{
		return new ExtendedDamageInfo
		{
			Position = hitPosition,
			Force = hitForce,
			Damage = damage,
			Tags = new HashSet<string> { "bullet" }
		};
	}

	/// <summary>
	/// Creates a new DamageInfo with no tags.
	/// </summary>
	/// <param name="damage"></param>
	/// <returns></returns>
	public static ExtendedDamageInfo Generic( float damage )
	{
		return new ExtendedDamageInfo
		{
			Damage = damage,
			Tags = new HashSet<string> { "generic" }
		};
	}

	/// <summary>
	/// Creates a new DamageInfo with the "explosion" tag.
	/// </summary>
	/// <param name="sourcePosition"></param>
	/// <param name="force"></param>
	/// <param name="damage"></param>
	/// <returns></returns>
	public static ExtendedDamageInfo FromExplosion( Vector3 sourcePosition, Vector3 force, float damage )
	{
		return new ExtendedDamageInfo
		{
			Origin = sourcePosition,
			Force = force,
			Damage = damage,
			Tags = new HashSet<string> { "explosion", "blast" }
		};
	}

	public ExtendedDamageInfo WithAttacker( Entity attacker )
	{
		Attacker = attacker;

		return this;
	}

	public ExtendedDamageInfo WithWeapon( Carriable weapon )
	{
		Weapon = weapon;

		return this;
	}

	public ExtendedDamageInfo WithTag( string tag )
	{
		Tags ??= new( StringComparer.OrdinalIgnoreCase );

		Tags.Add( tag );

		return this;
	}

	public ExtendedDamageInfo WithTags( params string[] tags )
	{
		Tags ??= new( StringComparer.OrdinalIgnoreCase );

		foreach ( var tag in tags )
		{
			Tags.Add( tag );
		}

		return this;
	}

	public bool HasTag( string tag )
	{
		return Tags?.Contains( tag ) ?? false;
	}

	public ExtendedDamageInfo WithHitBody( PhysicsBody body )
	{
		Body = body;

		return this;
	}

	public ExtendedDamageInfo WithHitbox( Hitbox hitbox )
	{
		Hitbox = hitbox;

		return this;
	}

	public ExtendedDamageInfo WithBone( int bone )
	{
		BoneIndex = bone;

		return this;
	}

	public ExtendedDamageInfo WithDamage( float damage )
	{
		Damage = damage;

		return this;
	}

	public ExtendedDamageInfo WithPosition( Vector3 position )
	{
		Position = position;

		return this;
	}

	/// <summary>
	/// The position from which this damage originated. I.e. the origin of an explosion that damaged the player.
	/// </summary>
	public ExtendedDamageInfo WithOrigin( Vector3 position )
	{
		Origin = position;

		return this;
	}

	public ExtendedDamageInfo WithForce( Vector3 force )
	{
		Force = force;

		return this;
	}

	public ExtendedDamageInfo UsingTraceResult( TraceResult result )
	{
		Position = result.EndPosition;
		Origin = result.StartPosition;
		Hitbox = result.Hitbox;
		BoneIndex = result.Bone;
		Body = result.Body;

		return this;
	}

	public DamageInfo ToDamageInfo()
	{
		return new DamageInfo()
		{
			Attacker = Attacker,
			Weapon = Weapon,
			Position = Position,
			Force = Force,
			Damage = Damage,
			Hitbox = Hitbox,
			BoneIndex = BoneIndex,
		};
	}
}
