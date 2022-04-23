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
	public float MaxHealth { get; set; } = 100f;
	public DamageInfo LastDamageInfo { get; private set; }
	public float LastDistanceToAttacker { get; set; } = 0f;
	private static readonly float ArmorReductionPercentage = 0.7f;

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
	/// If a player does not damage team members in a round, he has a "clean" round
	/// and gets a bonus for it.
	/// </summary>
	public bool CleanRound { get; set; } = true;

	/// <summary>
	/// The live karma starts equal to the base karma, but is updated "live" as the
	/// player damages/kills others. When a player damages/kills another, the
	/// live karma is used to determine his karma penalty.
	/// </summary>
	public float CurrentKarma { get; set; }

	public struct HealthGroup
	{
		public string Title;
		public Color Color;
		public int MinHealth;

		public HealthGroup( string title, Color color, int minHealth )
		{
			Title = title;
			Color = color;
			MinHealth = minHealth;
		}
	}

	private static readonly HealthGroup[] HealthGroupList = new HealthGroup[]
	{
		new HealthGroup("Near Death", Color.FromBytes(246, 6, 6), 0),
		new HealthGroup("Badly Wounded", Color.FromBytes(234, 129, 4), 20),
		new HealthGroup("Wounded", Color.FromBytes(213, 202, 4), 40),
		new HealthGroup("Hurt", Color.FromBytes(171, 231, 3), 60),
		new HealthGroup("Healthy", Color.FromBytes(44, 233, 44), 80)
	};

	public HealthGroup GetHealthGroup( float health )
	{
		if ( Health > MaxHealth )
			return HealthGroupList[4];

		int index = (int)((health - 1f) / (MaxHealth / 5f));
		return HealthGroupList[index];
	}

	public void SetHealth( float health )
	{
		Host.AssertServer();

		Health = Math.Min( health, MaxHealth );
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.Attacker is Player attacker && attacker != this )
		{
			if ( Game.Current.Round is not InProgressRound and not PostRound )
				return;

			if ( info.Flags != DamageFlags.Slash )
				info.Damage *= attacker.DamageFactor;
		}

		if ( info.Flags == DamageFlags.Bullet )
			info.Damage *= GetBulletDamageMultipliers( info );

		var damageLocation = info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.IsValid() ? info.Attacker.Position : Position;
		OnDamageTaken( To.Single( Client ), damageLocation );

		LastDamageInfo = info;

		if ( Game.Current.Round is InProgressRound )
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
			damageMultiplier *= ArmorReductionPercentage;

		return damageMultiplier;
	}

	[ClientRpc]
	public void OnDamageTaken( Vector3 position )
	{
		UI.DamageIndicator.Instance?.OnHit( position );
		UI.PlayerInfo.Instance?.OnHit();
	}
}
