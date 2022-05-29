using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TTT;

public enum FireMode
{
	Automatic = 0,
	Semi = 1,
	Burst = 2
}

[GameResource( "Weapon", "weapon", "TTT weapon template.", Icon = "🔫" )]
public class WeaponInfo : CarriableInfo
{
	[Category( "Sounds" ), ResourceType( "sound" )]
	public string FireSound { get; set; } = "";

	[Category( "Sounds" ), ResourceType( "sound" )]
	public string DryFireSound { get; set; } = "";

	[Category( "Important" )]
	[Description( "The ammo type. Set this to None if you want the ammo for your weapon to be limited (you can't pick up or drop ammo for it)." )]
	public AmmoType AmmoType { get; set; } = AmmoType.None;

	[Category( "Important" )]
	public FireMode FireMode { get; set; } = FireMode.Automatic;

	[Category( "Stats" )]
	[Description( "The amount of bullets that come out in one shot." )]
	public int BulletsPerFire { get; set; } = 1;

	[Category( "Stats" )]
	public int ClipSize { get; set; } = 30;

	[Category( "Stats" )]
	public float Damage { get; set; } = 20;

	[Category( "Stats" )]
	public float DamageFallOffStart { get; set; } = 0f;

	[Category( "Stats" )]
	public float DamageFallOffEnd { get; set; } = 0f;

	[Category( "Stats" )]
	public float HeadshotMultiplier { get; set; } = 1f;

	[Category( "Stats" )]
	public float Spread { get; set; } = 0f;

	[Category( "Stats" )]
	public float PrimaryRate { get; set; } = 0f;

	[Category( "Stats" )]
	public float SecondaryRate { get; set; } = 0f;

	[Category( "Stats" )]
	[Description( "The amount of ammo this weapon spawns with if the ammo type is set to None." )]
	public int ReserveAmmo { get; set; } = 0;

	[Category( "Stats" )]
	public float ReloadTime { get; set; } = 2f;

	[Category( "Stats" )]
	public float VerticalRecoil { get; set; } = 0f;

	[Category( "Stats" )]
	public float HorizontalRecoilRange { get; set; } = 0f;

	[Category( "Stats" )]
	public float RecoilRecoveryScale { get; set; } = 0f;

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string EjectParticle { get; set; } = "";

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string MuzzleFlashParticle { get; set; } = "";

	[Category( "VFX" ), ResourceType( "vpcf" )]
	public string TracerParticle { get; set; } = "";

	protected override void PostLoad()
	{
		base.PostLoad();

		Precache.Add( EjectParticle );
		Precache.Add( MuzzleFlashParticle );
		Precache.Add( TracerParticle );
	}
}

[Title( "Weapon" ), Icon( "sports_martial_arts" )]
public abstract partial class Weapon : Carriable
{
	[Net, Predicted]
	public int AmmoClip { get; protected set; }

	[Net, Predicted]
	public int ReserveAmmo { get; protected set; }

	[Net, Local, Predicted]
	public bool IsReloading { get; protected set; }

	[Net, Local, Predicted]
	public TimeSince TimeSincePrimaryAttack { get; protected set; }

	[Net, Local, Predicted]
	public TimeSince TimeSinceSecondaryAttack { get; protected set; }

	[Net, Local, Predicted]
	public TimeSince TimeSinceReload { get; protected set; }

	public new WeaponInfo Info => (WeaponInfo)base.Info;
	public override string SlotText => $"{AmmoClip} + {ReserveAmmo + Owner?.AmmoCount( Info.AmmoType )}";
	public TimeSince TimeSinceLastClientShoot { get; private set; }

	private Vector3 RecoilOnShoot => new( Rand.Float( -Info.HorizontalRecoilRange, Info.HorizontalRecoilRange ), Info.VerticalRecoil, 0 );
	private Vector3 CurrentRecoil { get; set; } = Vector3.Zero;

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = Info.ClipSize;

		if ( Info.AmmoType == AmmoType.None )
			ReserveAmmo = Info.ReserveAmmo;
	}

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		IsReloading = false;
		TimeSinceReload = 0;
	}

	public override void Simulate( Client client )
	{
		if ( CanReload() )
		{
			Reload();
			return;
		}

		if ( !IsReloading )
		{
			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}

			if ( CanSecondaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSinceSecondaryAttack = 0;
					AttackSecondary();
				}
			}

			if ( Input.Down( InputButton.Run ) && Input.Pressed( InputButton.Drop ) )
				DropAmmo();
		}
		else if ( TimeSinceReload > Info.ReloadTime )
			OnReloadFinish();
	}

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		float oldPitch = input.ViewAngles.pitch;
		float oldYaw = input.ViewAngles.yaw;

		input.ViewAngles.pitch -= CurrentRecoil.y * Time.Delta;
		input.ViewAngles.yaw -= CurrentRecoil.x * Time.Delta;

		CurrentRecoil -= CurrentRecoil
			.WithY( (oldPitch - input.ViewAngles.pitch) * Info.RecoilRecoveryScale )
			.WithX( (oldYaw - input.ViewAngles.yaw) * Info.RecoilRecoveryScale );
	}

	protected virtual bool CanPrimaryAttack()
	{
		if ( Info.FireMode == FireMode.Semi && !Input.Pressed( InputButton.PrimaryAttack ) )
			return false;
		else if ( Info.FireMode != FireMode.Semi && !Input.Down( InputButton.PrimaryAttack ) )
			return false;

		float rate = Info.PrimaryRate;
		if ( rate <= 0 )
			return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	protected virtual bool CanSecondaryAttack()
	{
		if ( !Input.Pressed( InputButton.SecondaryAttack ) )
			return false;

		float rate = Info.SecondaryRate;
		if ( rate <= 0 )
			return true;

		return TimeSinceSecondaryAttack > (1 / rate);
	}

	protected virtual void AttackPrimary()
	{
		if ( AmmoClip == 0 )
		{
			DryFireEffects();
			PlaySound( Info.DryFireSound );

			return;
		}

		AmmoClip--;

		Owner.SetAnimParameter( "b_attack", true );
		ShootEffects();
		PlaySound( Info.FireSound );

		ShootBullet( Info.Spread, 1.5f, Info.Damage, 3.0f, Info.BulletsPerFire );
	}

	protected virtual void AttackSecondary() { }

	protected virtual bool CanReload()
	{
		if ( IsReloading )
			return false;

		if ( !Input.Pressed( InputButton.Reload ) )
			return false;

		if ( AmmoClip >= Info.ClipSize || (Owner.AmmoCount( Info.AmmoType ) <= 0 && ReserveAmmo <= 0) )
			return false;

		return true;
	}

	protected virtual void Reload()
	{
		if ( IsReloading )
			return;

		TimeSinceReload = 0;
		IsReloading = true;

		Owner.SetAnimParameter( "b_reload", true );
		ReloadEffects();
	}

	protected virtual void OnReloadFinish()
	{
		IsReloading = false;
		AmmoClip += TakeAmmo( Info.ClipSize - AmmoClip );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		if ( !string.IsNullOrEmpty( Info.MuzzleFlashParticle ) )
			Particles.Create( Info.MuzzleFlashParticle, EffectEntity, "muzzle" );

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CurrentRecoil += RecoilOnShoot;
		TimeSinceLastClientShoot = 0;
	}

	[ClientRpc]
	protected virtual void DryFireEffects()
	{
		ViewModelEntity?.SetAnimParameter( "dryfire", true );
	}

	[ClientRpc]
	protected virtual void ReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );
	}

	protected virtual void ShootBullet( float spread, float force, float damage, float bulletSize, int bulletCount )
	{
		// Seed rand using the tick, so bullet cones match on client and server
		Rand.SetSeed( Time.Tick );

		while ( bulletCount-- > 0 )
		{
			var forward = Owner.EyeRotation.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			foreach ( var trace in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 20000f, bulletSize ) )
			{
				trace.Surface.DoBulletImpact( trace );

				var fullEndPosition = trace.EndPosition + trace.Direction * bulletSize;

				if ( !string.IsNullOrEmpty( Info.TracerParticle ) && trace.Distance > 200 )
				{
					var tracer = Particles.Create( Info.TracerParticle );
					tracer?.SetPosition( 0, trace.StartPosition );
					tracer?.SetPosition( 1, trace.EndPosition );
				}

				if ( !IsServer )
					continue;

				if ( !trace.Entity.IsValid() )
					continue;

				using ( Prediction.Off() )
				{
					OnHit( trace );

					if ( Info.Damage <= 0 )
						continue;

					var damageInfo = DamageInfo.FromBullet( trace.EndPosition, forward * 100f * force, damage )
						.UsingTraceResult( trace )
						.WithAttacker( Owner )
						.WithWeapon( this );

					if ( trace.Entity is Player player )
						player.DistanceToAttacker = Vector3.DistanceBetween( Owner.Position, player.Position ).SourceUnitsToMeters();

					trace.Entity.TakeDamage( damageInfo );
				}
			}
		}
	}

	/// <summary>
	/// Called when the bullet hits something, i.e the World or an entity.
	/// </summary>
	/// <param name="trace"></param>
	protected virtual void OnHit( TraceResult trace ) { }

	/// <summary>
	/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
	/// hits, like if you're going through layers or ricocet'ing or something.
	/// </summary>
	protected IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool InWater = Map.Physics.IsPointWater( start );

		var trace = Trace.Ray( start, end )
			.UseHitboxes()
			.HitLayer( CollisionLayer.Water, !InWater )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( Owner )
			.Ignore( this )
			.Size( radius )
			.Run();

		yield return trace;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}

	protected void DropAmmo()
	{
		if ( Info.AmmoType == AmmoType.None || AmmoClip <= 0 )
			return;

		if ( IsServer )
		{
			var ammoCrate = Ammo.Create( Info.AmmoType, AmmoClip );
			ammoCrate.Position = Owner.EyePosition + Owner.EyeRotation.Forward * 40;
			ammoCrate.Rotation = Owner.EyeRotation;
			ammoCrate.PhysicsGroup.Velocity = Owner.Velocity + Owner.EyeRotation.Forward * Player.DropVelocity;
			ammoCrate.Dropper = Owner;
		}

		AmmoClip = 0;
	}

	protected int TakeAmmo( int ammo )
	{
		int available = Math.Min( Info.AmmoType == AmmoType.None ? ReserveAmmo : Owner.AmmoCount( Info.AmmoType ), ammo );

		if ( Info.AmmoType == AmmoType.None )
			ReserveAmmo -= available;
		else
			Owner.TakeAmmo( Info.AmmoType, available );

		return available;
	}

	public static float GetDamageFalloff( float distance, float damage, float start, float end )
	{
		if ( end > 0f )
		{
			if ( start > 0f )
			{
				if ( distance < start )
					return damage;

				float falloffRange = end - start;
				float difference = (distance - start);

				return Math.Max( damage - (damage / falloffRange) * difference, 0f );
			}

			return Math.Max( damage - (damage / end) * distance, 0f );
		}

		return damage;
	}
}
