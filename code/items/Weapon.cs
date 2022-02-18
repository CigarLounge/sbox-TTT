using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace TTT.Items;

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
	[Property, Category( "Stats" )] public int ClipSize { get; set; } = 30;
	[Property, Category( "Stats" )] public float PrimaryRate { get; set; }
	[Property, Category( "Stats" )] public float SecondaryRate { get; set; }
	[Property, Category( "Stats" )] public float ReloadTime { get; set; } = 2f;
	[Property, Category( "Stats" )] public int ReserveAmmo { get; set; }
	[Property, Category( "VFX" ), ResourceType( "vpcf" )] public string EjectParticle { get; set; } = "particles/pistol_ejectbrass.vpcf";
	[Property, Category( "VFX" ), ResourceType( "vpcf" )] public string MuzzleFlashParticle { get; set; } = "particles/swb/muzzle/flash_medium.vpcf";
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

	public void AttackPrimary() { }

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
		IsReloading = false;
		AmmoClip += TakeAmmo( Info.ClipSize - AmmoClip );
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		// Particles.Create( "particles/impact.generic.smokering.vpcf", EffectEntity, "muzzle" );

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

	/// <summary>
	/// Does a trace from start to end, does bullet impact effects. Coded as an IEnumerable so you can return multiple
	/// hits, like if you're going through layers or ricocet'ing or something.
	/// </summary>
	public virtual IEnumerable<TraceResult> TraceBullet( Vector3 start, Vector3 end, float radius = 2.0f )
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
}
