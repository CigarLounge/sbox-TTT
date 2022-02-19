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

[Library( "weap" ), AutoGenerate]
public partial class WeaponInfo : CarriableInfo
{
	[Property, Category( "Sounds" )] public string FireSound { get; set; } = "";
	[Property, Category( "Sounds" )] public string DryFireSound { get; set; } = "";
	[Property, Category( "Important" )] public FireMode FireMode { get; set; }
	[Property, Category( "Stats" )] public int BulletsPerFire { get; set; } = 1;
	[Property, Category( "Stats" )] public int ClipSize { get; set; } = 30;
	[Property, Category( "Stats" )] public float Damage { get; set; } = 20;
	[Property, Category( "Stats" )] public float Spread { get; set; } = 0f;
	[Property, Category( "Stats" )] public float PrimaryRate { get; set; }
	[Property, Category( "Stats" )] public float SecondaryRate { get; set; }
	[Property, Category( "Stats" )] public float ReloadTime { get; set; } = 2f;
	[Property, Category( "Stats" )] public int ReserveAmmo { get; set; }
	[Property, Category( "VFX" ), ResourceType( "vpcf" )] public string EjectParticle { get; set; } = "particles/pistol_ejectbrass.vpcf";
	[Property, Category( "VFX" ), ResourceType( "vpcf" )] public string MuzzleFlashParticle { get; set; }
}

public abstract partial class Weapon : Carriable
{
	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; protected set; }

	[Net, Predicted]
	public int ReserveAmmo { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSincePrimaryAttack { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceSecondaryAttack { get; protected set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; protected set; }

	public new WeaponInfo Info
	{
		get => base.Info as WeaponInfo;
		set => base.Info = value;
	}

	public bool UnlimitedAmmo { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		AmmoClip = Info.ClipSize;
		ReserveAmmo = Info.ReserveAmmo;
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < Info.DeployTime )
			return;

		if ( !IsReloading )
		{
			if ( CanReload() )
			{
				Reload();
			}

			//
			// Reload could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanPrimaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSincePrimaryAttack = 0;
					AttackPrimary();
				}
			}

			//
			// AttackPrimary could have changed our owner
			//
			if ( !Owner.IsValid() )
				return;

			if ( CanSecondaryAttack() )
			{
				using ( LagCompensation() )
				{
					TimeSinceSecondaryAttack = 0;
					AttackSecondary();
				}
			}
		}
		else if ( IsReloading && TimeSinceReload > Info.ReloadTime )
			OnReloadFinish();
	}

	public virtual bool CanPrimaryAttack()
	{
		if ( Info.FireMode == FireMode.Semi && !Input.Pressed( InputButton.Attack1 ) )
			return false;
		else if ( Info.FireMode != FireMode.Semi && !Input.Down( InputButton.Attack1 ) )
			return false;

		var rate = Info.PrimaryRate;
		if ( rate <= 0 )
			return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual bool CanSecondaryAttack()
	{
		if ( !Input.Pressed( InputButton.Attack2 ) )
			return false;

		var rate = Info.SecondaryRate;
		if ( rate <= 0 )
			return true;

		return TimeSinceSecondaryAttack > (1 / rate);
	}

	public void AttackPrimary()
	{
		if ( AmmoClip == 0 )
		{
			PlaySound( Info.DryFireSound );
			return;
		}

		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
		AmmoClip -= 1;

		Rand.SetSeed( Time.Tick );

		ShootEffects();
		PlaySound( Info.FireSound );
		for ( int i = 0; i < Info.BulletsPerFire; i++ )
		{
			ShootBullet( Info.Spread, 1.5f, Info.Damage, 3.0f );
		}
	}

	public void AttackSecondary() { }

	public virtual bool CanReload()
	{
		if ( AmmoClip >= Info.ClipSize || (!UnlimitedAmmo && ReserveAmmo == 0) )
			return false;

		if ( !Owner.IsValid() || !Input.Down( InputButton.Reload ) )
			return false;

		return true;
	}

	public virtual void Reload()
	{
		if ( IsReloading )
			return;

		TimeSinceReload = 0;
		IsReloading = true;

		Owner.SetAnimBool( "b_reload", true );
		ReloadEffects();
	}

	public virtual void OnReloadFinish()
	{
		Log.Debug();
		IsReloading = false;
		AmmoClip += TakeAmmo( Info.ClipSize - AmmoClip );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Particles.Create( Info.EjectParticle, EffectEntity, "muzzle" );

		if ( IsLocalPawn )
			_ = new Sandbox.ScreenShake.Perlin( 1f, 0.2f, 0.8f );

		ViewModelEntity?.SetAnimBool( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	[ClientRpc]
	protected virtual void ReloadEffects()
	{
		ViewModelEntity?.SetAnimBool( "reload", true );
	}

	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize )
	{
		var forward = Owner.EyeRotation.Forward;
		forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
		forward = forward.Normal;

		foreach ( var trace in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * 20000f, bulletSize ) )
		{
			trace.Surface.DoBulletImpact( trace );

			var fullEndPos = trace.EndPos + trace.Direction * bulletSize;
			/*
			if ( !string.IsNullOrEmpty( TracerEffect ) )
			{
				var tracer = Particles.Create( TracerEffect, GetEffectEntity(), MuzzleAttachment );
				tracer?.SetPosition( 1, fullEndPos );
				tracer?.SetPosition( 2, trace.Distance );
			}

			if ( !string.IsNullOrEmpty( ImpactEffect ) )
			{
				var impact = Particles.Create( ImpactEffect, fullEndPos );
				impact?.SetForward( 0, trace.Normal );
			}
			*/
			if ( !IsServer )
				continue;

			//WeaponUtil.PlayFlybySounds( Owner, trace.Entity, trace.StartPos, trace.EndPos, bulletSize * 2f, bulletSize * 50f, FlybySounds );

			if ( trace.Entity.IsValid() )
			{
				using ( Prediction.Off() )
				{
					var damageInfo = new DamageInfo()
						.WithPosition( trace.EndPos )
						.WithFlag( DamageFlags.Bullet )
						.WithForce( forward * 100f * force )
						.UsingTraceResult( trace )
						.WithAttacker( Owner )
						.WithWeapon( this );

					damageInfo.Damage = Info.Damage;
					//GetDamageFalloff( trace.Distance, damage, trace.StartPos, trace.EndPos );

					trace.Entity.TakeDamage( damageInfo );
				}
			}
		}
	}

	/// <summary>
	/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
	/// hits, like if you're going through layers or ricocet'ing or something.
	/// </summary>
	public IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool InWater = Physics.TestPointContents( start, CollisionLayer.Water );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.HitLayer( CollisionLayer.Debris )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		yield return tr;

		//
		// Another trace, bullet going through thin material, penetrating water surface?
		//
	}

	protected int TakeAmmo( int ammo )
	{
		if ( UnlimitedAmmo )
			return ammo;

		int available = Math.Min( ReserveAmmo, ammo );
		ReserveAmmo -= available;

		return available;
	}

	public static float GetDamageFalloff( float distance, float damage, float start, float end )
	{
		if ( end > 0f )
		{
			if ( start > 0f )
			{
				if ( distance < start )
				{
					return damage;
				}

				var falloffRange = end - start;
				var difference = (distance - start);

				return Math.Max( damage - (damage / falloffRange) * difference, 0f );
			}

			return Math.Max( damage - (damage / end) * distance, 0f );
		}

		return damage;
	}
}
