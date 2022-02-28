using System;
using System.Collections.Generic;
using Sandbox;

namespace TTT;

public enum AmmoType : byte
{
	/// <summary>
	/// Used for weapons that cannot pickup any additional ammo.
	/// </summary>
	None,
	PistolSMG,
	Shotgun,
	Sniper,
	Magnum,
	Rifle
}

[Hammer.Skip]
public abstract partial class Ammo : Prop, IEntityHint, IUse
{
	///<summary>How much ammo is left in this box.</summary>
	[Net]
	public int CurrentCount { get; set; }

	public virtual AmmoType Type => AmmoType.None;

	///<summary>The ammo box will spawn with this much in it.</summary>
	public virtual int DefaultAmmoCount => 30;
	///<summary>The model for the ammo box.</summary>
	protected virtual string WorldModelPath => string.Empty;

	public static Ammo Create( AmmoType ammoType, int count = 0 )
	{
		Host.AssertServer();

		Ammo ammo = ammoType switch
		{
			AmmoType.None => null,
			AmmoType.PistolSMG => new SMGAmmo(),
			AmmoType.Shotgun => new ShotgunAmmo(),
			AmmoType.Sniper => new SniperAmmo(),
			AmmoType.Magnum => new MagnumAmmo(),
			AmmoType.Rifle => new RifleAmmo(),
			_ => null,
		};


		if ( ammo == null )
			return null;

		ammo.CurrentCount = count == 0 ? ammo.DefaultAmmoCount : count;

		return ammo;
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
		CurrentCount = DefaultAmmoCount;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other is Player player )
			GiveAmmo( player );
	}

	private void GiveAmmo( Player player )
	{
		if ( !player.Inventory.HasWeaponOfAmmoType( Type ) )
			return;

		int iMaxAmmo = Inventory.GetAmmoLimit( Type );
		int iAdd = Math.Min( CurrentCount, iMaxAmmo - player.AmmoCount( Type ) );
		if ( iAdd <= 0 ) return;

		player.GiveAmmo( Type, iAdd );
		PlaySound( RawStrings.AmmoPickupSound );

		CurrentCount -= iAdd;

		// Delete empty/nearly empty ammo boxes.
		if ( CurrentCount < Math.Ceiling( DefaultAmmoCount * 0.25f ) )
			Delete();
	}

	string IEntityHint.TextOnTick => $"{Type} Ammo";
	public float HintDistance => Player.INTERACT_DISTANCE;

	bool IEntityHint.CanHint( Player player )
	{
		return true;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Hint( (this as IEntityHint).TextOnTick );
	}

	bool IUse.OnUse( Entity user )
	{
		GiveAmmo( user as Player );

		return false;
	}

	bool IUse.IsUsable( Entity user )
	{
		return user.IsAlive() && user is Player;
	}
}
