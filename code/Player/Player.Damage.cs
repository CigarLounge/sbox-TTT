using Sandbox;
using System;

namespace TTT;

/*
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
*/

public struct ColorGroup
{
	public Color Color;
	public string Title;

	public ColorGroup( string title, Color color )
	{
		Title = title;
		Color = color;
	}
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
	public const float MaxHealth = 100f;
	public DamageInfo LastDamageInfo { get; private set; }
	public float DistanceToAttacker { get; set; }

	public new float Health
	{
		get => base.Health;
		set => base.Health = Math.Min( value, MaxHealth );
	}

	/// <summary>
	/// We count all player deaths not caused by other players as suicides.
	/// </summary>
	public bool DiedBySuicide => LastAttacker is not Player || LastAttacker == this;

	/// <summary>
	/// The base/start karma is determined once per round and determines the player's
	/// damage penalty. It is networked and shown on clients.
	/// </summary>
	public float BaseKarma
	{
		get => Client.GetValue<float>( Strings.Karma );
		set => Client.SetValue( Strings.Karma, value );
	}

	/// <summary>
	/// The damage factor scales how much damage the player deals, so if it is 0.9
	/// then the player only deals 90% of his original damage.
	/// </summary>
	public float DamageFactor { get; set; } = 1f;

	/// <summary>
	/// If a player damages another team member that is "clean" (no active timer),
	/// they'll end up with time being tacked onto this timer. A player will receive a
	/// karma bonus for remaining "clean" (having no active timer) at the end of the round.
	/// </summary>
	public TimeUntil TimeUntilClean { get; set; } = 0f;

	/// <summary>
	/// The active karma starts equal to the base karma, but is updated as the
	/// player damages/kills others. When a player damages/kills another, the
	/// active karma is used to determine his karma penalty.
	/// </summary>
	public float ActiveKarma { get; set; }

	private static readonly ColorGroup[] HealthGroupList = new ColorGroup[]
	{
		new ColorGroup("Near Death", Color.FromBytes(246, 6, 6)),
		new ColorGroup("Badly Wounded", Color.FromBytes(234, 129, 4)),
		new ColorGroup("Wounded", Color.FromBytes(213, 202, 4)),
		new ColorGroup("Hurt", Color.FromBytes(171, 231, 3)),
		new ColorGroup("Healthy", Color.FromBytes(44, 233, 44))
	};

	public ColorGroup GetHealthGroup( float health )
	{
		if ( Health > MaxHealth )
			return HealthGroupList[^1];

		int index = (int)((health - 1f) / (MaxHealth / HealthGroupList.Length));
		return HealthGroupList[index];
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.Attacker is Player attacker && attacker != this )
		{
			if ( Game.Current.State is not InProgress and not PostRound )
				return;

			if ( info.Flags != DamageFlags.Slash )
				info.Damage *= attacker.DamageFactor;
		}

		if ( info.Flags == DamageFlags.Bullet )
			info.Damage *= GetBulletDamageMultipliers( info );

		var damageLocation = info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.IsValid() ? info.Attacker.Position : Position;
		OnDamageTaken( To.Single( Client ), damageLocation );

		LastDamageInfo = info;

		if ( Game.Current.State is InProgress )
			Karma.OnPlayerHurt( this );

		base.TakeDamage( info );
	}

	private float GetBulletDamageMultipliers( DamageInfo info )
	{
		var damageMultiplier = 1f;

		var isHeadShot = (HitboxGroup)GetHitboxGroup( info.HitboxIndex ) == HitboxGroup.Head;
		if ( isHeadShot )
		{
			var weaponInfo = Asset.GetInfo<WeaponInfo>( info.Weapon );
			if ( weaponInfo is not null )
				damageMultiplier *= weaponInfo.HeadshotMultiplier;
		}

		if ( !isHeadShot && Perks.Has( typeof( BodyArmor ) ) )
			damageMultiplier *= BodyArmor.ReductionPercentage;

		return damageMultiplier;
	}

	[ClientRpc]
	public void OnDamageTaken( Vector3 position )
	{
		UI.DamageIndicator.Instance?.OnHit( position );
		UI.PlayerInfo.Instance?.OnHit();
	}
}
