using System;

using Sandbox;

namespace TTT;

public enum HitboxIndex
{
	Pelvis = 1,
	Stomach = 2,
	Rips = 3,
	Neck = 4,
	Head = 5,
	LeftUpperArm = 7,
	LeftLowerArm = 8,
	LeftHand = 9,
	RightUpperArm = 11,
	RightLowerArm = 12,
	RightHand = 13,
	RightUpperLeg = 14,
	RightLowerLeg = 15,
	RightFoot = 16,
	LeftUpperLeg = 17,
	LeftLowerLeg = 18,
	LeftFoot = 19,
}

public enum HitboxGroup
{
	None = -1,
	Generic = 0,
	Head = 1,
	Chest = 2,
	Stomach = 3,
	LeftArm = 4,
	RightArm = 5,
	LeftLeg = 6,
	RightLeg = 7,
	Gear = 10,
	Special = 11,
}

public partial class Player
{
	[Net]
	public float MaxHealth { get; set; } = 100f;

	public Carriable LastDamageWeapon { get; private set; }

	public bool LastDamageWasHeadshot { get; private set; } = false;

	public float LastDistanceToAttacker { get; private set; } = 0f;

	public float ArmorReductionPercentage { get; private set; } = 0.7f;

	public void SetHealth( float health )
	{
		Health = Math.Min( health, MaxHealth );
	}

	public override void TakeDamage( DamageInfo info )
	{
		LastDamageWasHeadshot = GetHitboxGroup( info.HitboxIndex ) == (int)HitboxGroup.Head;

		if ( LastDamageWasHeadshot )
		{
			info.Damage *= 2.0f;
		}

		if ( Perks.Has( typeof( BodyArmor ) ) && !LastDamageWasHeadshot && (info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet )
		{
			info.Damage *= ArmorReductionPercentage;
		}

		LastDamageWeapon = info.Weapon.IsValid() ? info.Weapon as Carriable : null;

		To client = To.Single( this );

		if ( info.Attacker is Player attacker && attacker != this )
		{
			LastDistanceToAttacker = Utils.SourceUnitsToMeters( Position.Distance( attacker.Position ) );

			if ( Game.Current.Round is not (InProgressRound or PostRound) )
			{
				return;
			}

			ClientAnotherPlayerDidDamage( client, info.Position, Health.LerpInverse( 100, 0 ) );
		}
		else
		{
			LastDistanceToAttacker = 0f;
		}

		ClientTookDamage( client, info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.IsValid() ? info.Attacker.Position : Position, info.Damage );

		// Play pain sounds
		if ( (info.Flags & DamageFlags.Fall) == DamageFlags.Fall )
		{
			var volume = 0.05f * info.Damage;
			PlaySound( "fall" ).SetVolume( volume > 0.5f ? 0.5f : volume ).SetPosition( info.Position );
		}
		else if ( (info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet )
		{
			PlaySound( "grunt" + Rand.Int( 1, 4 ) ).SetVolume( 0.4f ).SetPosition( info.Position );
		}

		_lastDamageInfo = info;

		base.TakeDamage( info );
	}
}
