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

public abstract partial class Ammo : Prop, IEntityHint, IUse
{
	[Net]
	public int CurrentCount { get; private set; }
	public Player Dropper { get; set; }
	public virtual AmmoType Type => AmmoType.None;
	public virtual int DefaultAmmoCount => 30;
	protected virtual string WorldModelPath => string.Empty;
	private TimeSince _timeSinceDropped = 0;

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

		if ( other is Player player && (player != Dropper || _timeSinceDropped >= 1f) )
			GiveAmmo( player );
	}

	private void GiveAmmo( Player player )
	{
		if ( !this.IsValid() || !player.Inventory.HasWeaponOfAmmoType( Type ) )
			return;

		int ammoPickedUp = player.GiveAmmo( Type, CurrentCount );
		CurrentCount -= ammoPickedUp;

		if ( CurrentCount <= 0 )
			Delete();
	}

	public string TextOnTick => $"{Type} Ammo x{CurrentCount}";
	public float HintDistance => Player.INTERACT_DISTANCE;

	bool IEntityHint.CanHint( Player player )
	{
		return true;
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player )
	{
		return new UI.Hint( TextOnTick );
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
