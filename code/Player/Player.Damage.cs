using Sandbox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

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

	public DamageInfo LastDamage { get; private set; }

	public new float Health
	{
		get => base.Health;
		set => base.Health = Math.Clamp( value, 0, MaxHealth );
	}

	public new Entity LastAttacker
	{
		get => base.LastAttacker;
		set
		{
			// If anyone uses a prop to kill someone.
			if ( value is Prop prop && prop.LastAttacker is Player propAttacker )
				base.LastAttacker = propAttacker;
			else
				base.LastAttacker = value;
		}
	}

	/// <summary>
	/// Whether or not they were killed by another Player.
	/// This includes if the Player used a prop to kill them.
	/// </summary>
	public bool KilledByPlayer => LastAttacker is Player && LastAttacker != this;

	/// <summary>
	/// Whether or not the player was killed with a head shot.
	/// </summary>
	public bool KilledWithHeadShot => LastDamage.Hitbox.HasTag( "head" );

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
		Game.AssertServer();

		LifeState = LifeState.Dead;
		Status = PlayerStatus.MissingInAction;
		TimeSinceDeath = 0;
		Client.AddInt( "deaths" );

		if ( KilledByPlayer )
		{
			LastAttacker.Client.AddInt( "kills" );
			(LastAttacker as Player).PlayersKilled.Add( this );
		}

		BecomeCorpse();
		RemoveAllDecals();
		StopUsing();

		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableTouch = false;

		Inventory.DropAll();
		DeleteFlashlight();
		DeleteItems();

		Event.Run( GameEvent.Player.Killed, this );
		GameManager.Current.State.OnPlayerKilled( this );

		ClientOnKilled( this );
	}

	private void ClientOnKilled()
	{
		Game.AssertClient();

		if ( IsLocalPawn )
		{
			CurrentChannel = Channel.Spectator;
			ClearButtons();
		}

		DeleteFlashlight();
		Event.Run( GameEvent.Player.Killed, this );
	}

	public override void TakeDamage( DamageInfo info )
	{
		Game.AssertServer();

		if ( !this.IsAlive() )
			return;

		if ( info.Attacker is Prop && info.Attacker.Tags.Has( Strings.Tags.IgnoreDamage ) )
			return;

		if ( info.Attacker is Player attacker && attacker != this )
		{
			if ( GameManager.Current.State is not InProgress and not PostRound )
				return;

			if ( !info.HasTag( Strings.Tags.Slash ) )
				info.Damage *= attacker.DamageFactor;
		}

		if ( info.HasTag( Strings.Tags.Bullet ) )
			info.Damage *= GetBulletDamageMultipliers( ref info );

		if ( info.HasTag( Strings.Tags.Blast ) )
			Deafen( To.Single( this ), info.Damage.LerpInverse( 0, 60 ) );

		info.Damage = Math.Min( Health, info.Damage );

		LastAttacker = info.Attacker;
		LastAttackerWeapon = info.Weapon;
		LastAttackerWeaponInfo = (info.Weapon as Carriable)?.Info;
		LastDamage = info;

		Health -= info.Damage;
		Event.Run( GameEvent.Player.TookDamage, this );

		SendDamageInfo( To.Single( this ) );

		this.ProceduralHitReaction( info );

		if ( Health <= 0f )
			OnKilled();
	}

	public void SendDamageInfo( To to )
	{
		Game.AssertServer();
		SendDamageInfo
		(
			to,
			LastAttacker,
			LastAttackerWeapon,
			LastAttackerWeaponInfo,
			LastDamage.Damage,
			LastDamage.Tags.ToArray(),
			LastDamage.Position,
			DistanceToAttacker
		);
	}

	private float GetBulletDamageMultipliers( ref DamageInfo info )
	{
		var damageMultiplier = 1f;

		if ( Perks.Has<Armor>() )
			damageMultiplier *= Armor.ReductionPercentage;

		if ( info.Hitbox.HasTag( "head" ) )
		{
			var weaponInfo = GameResource.GetInfo<WeaponInfo>( info.Weapon.ClassName );
			damageMultiplier *= weaponInfo?.HeadshotMultiplier ?? 2f;
		}
		else if ( info.Hitbox.HasAnyTags( "arm", "hand" ) )
		{
			damageMultiplier *= 0.55f;
		}

		return damageMultiplier;
	}

	private void ResetDamageData()
	{
		DistanceToAttacker = 0;
		LastAttacker = null;
		LastAttackerWeapon = null;
		LastAttackerWeaponInfo = null;
		LastDamage = default;
	}

	[ClientRpc]
	public static void Deafen( float strength )
	{
		Audio.SetEffect( "flashbang", strength, velocity: 20.0f, fadeOut: 4.0f * strength );
	}

	[ClientRpc]
	private void SendDamageInfo( Entity a, Entity w, CarriableInfo wI, float d, string[] tags, Vector3 p, float dTA )
	{
		var info = DamageInfo.Generic( d )
			.WithAttacker( a )
			.WithWeapon( w )
		   .WithPosition( p );

		info.Tags = new HashSet<string>( tags );

		DistanceToAttacker = dTA;
		LastAttacker = a;
		LastAttackerWeapon = w;
		LastAttackerWeaponInfo = wI;
		LastDamage = info;

		if ( IsLocalPawn )
			Event.Run( GameEvent.Player.TookDamage, this );
	}

	[ClientRpc]
	public static void ClientOnKilled( Player player )
	{
		if ( !player.IsValid() )
			return;

		player.ClientOnKilled();
	}
}
