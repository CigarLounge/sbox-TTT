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

	public virtual AmmoType Type => AmmoType.None;
	public virtual int DefaultAmmoCount => 30;
	protected virtual string WorldModelPath => string.Empty;

	private Player _dropper;
	private TimeSince _timeSincedDropped;

	public static Ammo Create( AmmoType ammoType, int count = 0 )
	{
		Host.AssertServer();

		var ammo = ammoType switch
		{
			AmmoType.None => null,
			AmmoType.PistolSMG => new SMGAmmo(),
			AmmoType.Shotgun => new ShotgunAmmo(),
			AmmoType.Sniper => new SniperAmmo(),
			AmmoType.Magnum => new MagnumAmmo(),
			AmmoType.Rifle => new RifleAmmo(),
			_ => default( Ammo ),
		};


		if ( ammo is null )
			return null;

		ammo.CurrentCount = count == 0 ? ammo.DefaultAmmoCount : count;

		return ammo;
	}

	public static Ammo Drop( Player dropper, AmmoType ammoType, int count = 0 )
	{
		var ammoCrate = Create( ammoType, count );
		ammoCrate.Position = dropper.EyePosition + dropper.EyeRotation.Forward * 40;
		ammoCrate.Rotation = dropper.EyeRotation;
		ammoCrate.PhysicsGroup.Velocity = dropper.Velocity + dropper.EyeRotation.Forward * Player.DropVelocity;
		ammoCrate._dropper = dropper;
		ammoCrate._timeSincedDropped = 0;
		return ammoCrate;
	}

	public override void Spawn()
	{
		Tags.Add( "trigger" );
		SetModel( WorldModelPath );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		PhysicsEnabled = true;
		UsePhysicsCollision = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;
		CurrentCount = DefaultAmmoCount;
	}

	public override void StartTouch( Entity other )
	{
		if ( !IsServer )
			return;

		if ( other is Player player && (player != _dropper || _timeSincedDropped >= 1f) )
			GiveAmmo( player );
	}

	private void GiveAmmo( Player player )
	{
		if ( !this.IsValid() || !player.Inventory.HasWeaponOfAmmoType( Type ) )
			return;

		var ammoPickedUp = player.GiveAmmo( Type, CurrentCount );
		CurrentCount -= ammoPickedUp;

		if ( CurrentCount <= 0 )
			Delete();
	}

	UI.EntityHintPanel IEntityHint.DisplayHint( Player player ) => new UI.Hint( $"{DisplayInfo.For( this ).Name} x{CurrentCount}" );

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
