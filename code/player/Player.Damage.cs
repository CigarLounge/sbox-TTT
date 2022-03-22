using Sandbox;
using System;

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

	public DamageInfo LastDamageInfo { get; private set; }

	public float LastDistanceToAttacker { get; set; } = 0f;

	public float ArmorReductionPercentage { get; private set; } = 0.7f;

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
		int index = (int)((health - 1f) / (MaxHealth / 5f));
		return HealthGroupList[index];
	}

	public void SetHealth( float health )
	{
		Health = Math.Min( health, MaxHealth );
	}

	public override void TakeDamage( DamageInfo info )
	{
		LastDamageInfo = info;

		HitboxGroup hitboxGroup = (HitboxGroup)GetHitboxGroup( info.HitboxIndex );
		if ( hitboxGroup == HitboxGroup.Head )
		{
			var weaponInfo = Asset.GetInfo<WeaponInfo>( info.Weapon );
			if ( weaponInfo != null )
				info.Damage *= weaponInfo.HeadshotMultiplier;
		}
		else if ( hitboxGroup == HitboxGroup.LeftArm || hitboxGroup == HitboxGroup.RightArm || hitboxGroup == HitboxGroup.LeftLeg || hitboxGroup == HitboxGroup.RightLeg )
		{
			info.Damage *= 0.7f;
		}
		else if ( Perks.Has( typeof( BodyArmor ) ) && (info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet )
		{
			info.Damage *= ArmorReductionPercentage;
		}

		if ( info.Attacker is Player attacker && attacker != this )
		{

			if ( Game.Current.Round is not (InProgressRound or PostRound) )
				return;

			ClientAnotherPlayerDidDamage( To.Single( Client ), info.Position, Health.LerpInverse( 100, 0 ) );
		}

		ClientTookDamage( To.Single( Client ), info.Weapon.IsValid() ? info.Weapon.Position : info.Attacker.IsValid() ? info.Attacker.Position : Position, info.Damage );

		if ( (info.Flags & DamageFlags.Fall) == DamageFlags.Fall )
		{
			var volume = 0.05f * info.Damage;
			PlaySound( "fall" ).SetVolume( volume > 0.5f ? 0.5f : volume ).SetPosition( info.Position );
		}
		else if ( (info.Flags & DamageFlags.Bullet) == DamageFlags.Bullet )
		{
			PlaySound( "grunt" + Rand.Int( 1, 4 ) ).SetVolume( 0.4f ).SetPosition( info.Position );
		}

		LastDamageInfo = info;

		base.TakeDamage( info );
	}

	[ClientRpc]
	public void ClientAnotherPlayerDidDamage( Vector3 position, float inverseHealth )
	{
		Sound.FromScreen( "dm.ui_attacker" )
			.SetPitch( 1 + inverseHealth * 1 )
			.SetPosition( position );
	}

	[ClientRpc]
	public void ClientTookDamage( Vector3 position, float damage )
	{
		UI.DamageIndicator.Instance?.OnHit( position );
		UI.PlayerInfo.Instance?.OnHit();
	}
}
