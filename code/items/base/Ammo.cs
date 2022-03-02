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
	[Net]
	public int CurrentCount { get; set; }

	public virtual AmmoType Type => AmmoType.None;

	public virtual int DefaultAmmoCount => 30;
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

		int ammoLimit = Inventory.GetAmmoLimit( Type );
		int playerAmmo = player.AmmoCount( Type );
		int addAmmo = Math.Min( CurrentCount, ammoLimit - playerAmmo );
		if ( addAmmo <= 0 ) return;

		player.GiveAmmo( Type, addAmmo );
		PlaySound( RawStrings.AmmoPickupSound );

		CurrentCount -= addAmmo;
		if ( CurrentCount <= 0 ) Delete();
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
