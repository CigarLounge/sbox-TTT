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

	[Net]
	public TimeSince TimeSinceDeath { get; private set; }

	public float DistanceToAttacker { get; set; }

	/// <summary>
	/// It's always better to use this than <see cref="Entity.LastAttackerWeapon"/>
	/// because the weapon may be invalid.
	/// </summary>
	public CarriableInfo LastAttackerWeaponInfo { get; private set; }

	public DamageInfo LastDamageInfo { get; private set; }

	public new float Health
	{
		get => base.Health;
		set => base.Health = Math.Clamp( value, 0, MaxHealth );
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

	private static readonly ColorGroup[] _healthGroupList = new ColorGroup[]
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
			return _healthGroupList[^1];

		var index = (int)((health - 1f) / (MaxHealth / _healthGroupList.Length));
		return _healthGroupList[index];
	}

	public override void OnKilled()
	{
		TimeSinceDeath = 0;
		LifeState = LifeState.Dead;
		StopUsing();

		Client.AddInt( "deaths" );

		if ( !DiedBySuicide )
			LastAttacker.Client.AddInt( "kills" );

		BecomeCorpse();
		RemoveAllDecals();

		EnableAllCollisions = false;
		EnableDrawing = false;

		Inventory.DropAll();
		DeleteFlashlight();
		DeleteItems();

		Event.Run( TTTEvent.Player.Killed, this );
		Game.Current.State.OnPlayerKilled( this );

		ClientOnKilled( this );
	}

	private void ClientOnKilled()
	{
		Host.AssertClient();

		if ( IsLocalPawn )
			ClearButtons();

		DeleteFlashlight();
		Event.Run( TTTEvent.Player.Killed, this );
	}

	public override void TakeDamage( DamageInfo info )
	{
		Host.AssertServer();

		if ( !this.IsAlive() )
			return;

		if ( info.Attacker is Player attacker && attacker != this )
		{
			if ( Game.Current.State is not InProgress and not PostRound )
				return;

			if ( !info.Flags.HasFlag( DamageFlags.Slash ) )
				info.Damage *= attacker.DamageFactor;
		}

		if ( info.Flags.HasFlag( DamageFlags.Bullet ) )
			info.Damage *= GetBulletDamageMultipliers( ref info );

		if ( info.Flags.HasFlag( DamageFlags.Blast ) )
			Deafen( To.Single( this ), info.Damage.LerpInverse( 0, 60 ) );

		info.Damage = Math.Min( Health, info.Damage );

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;
		LastAttackerWeaponInfo = info.Weapon is Carriable carriable ? carriable.Info : null;
		LastDamageInfo = info;

		Health -= info.Damage;
		Event.Run( TTTEvent.Player.TookDamage, this );

		SendDamageInfo( To.Single( this ) );

		this.ProceduralHitReaction( info );

		if ( Health <= 0f )
			OnKilled();
	}

	public void SendDamageInfo( To to )
	{
		Host.AssertServer();

		SendDamageInfo
		(
			to,
			LastAttacker,
			LastAttackerWeapon,
			LastAttackerWeaponInfo,
			LastDamageInfo.Damage,
			LastDamageInfo.Flags,
			LastDamageInfo.HitboxIndex,
			LastDamageInfo.Position,
			DistanceToAttacker
		);
	}

	private float GetBulletDamageMultipliers( ref DamageInfo info )
	{
		var damageMultiplier = 1f;
		var isHeadShot = (HitboxGroup)GetHitboxGroup( info.HitboxIndex ) == HitboxGroup.Head;

		if ( isHeadShot )
		{
			var weaponInfo = GameResource.GetInfo<WeaponInfo>( info.Weapon.ClassName );

			if ( weaponInfo is not null )
				damageMultiplier *= weaponInfo.HeadshotMultiplier;
		}
		else if ( Perks.Has<BodyArmor>() )
			damageMultiplier *= BodyArmor.ReductionPercentage;

		return damageMultiplier;
	}

	[ClientRpc]
	public static void Deafen( float strength )
	{
		Audio.SetEffect( "flashbang", strength, velocity: 20.0f, fadeOut: 4.0f * strength );
	}

	[ClientRpc]
	private void SendDamageInfo( Entity attacker, Entity weapon, CarriableInfo weaponInfo, float damage, DamageFlags damageFlag, int hitboxIndex, Vector3 position, float distanceToAttacker )
	{
		var info = DamageInfo.Generic( damage )
			.WithAttacker( attacker )
			.WithWeapon( weapon )
			.WithFlag( damageFlag )
			.WithHitbox( hitboxIndex )
			.WithPosition( position );

		DistanceToAttacker = distanceToAttacker;
		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;
		LastAttackerWeaponInfo = weaponInfo;
		LastDamageInfo = info;

		if ( IsLocalPawn )
			Event.Run( TTTEvent.Player.TookDamage, this );
	}

	[ClientRpc]
	public static void ClientOnKilled( Player player )
	{
		if ( !player.IsValid() )
			return;

		player.ClientOnKilled();
	}
}
