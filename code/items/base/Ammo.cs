using Sandbox;

namespace TTT;

public enum AmmoType : byte
{
	/// <summary>
	/// Use this for weapons that cannot have more ammo
	/// than what they spawned with.
	/// </summary>
	None,
	PistolSMG,
	Shotgun,
	Sniper,
	Magnum,
	Rifle
}

// TODO: Kole add hammer property for different world models.
[Hammer.Skip]
public abstract partial class Ammo : Prop, IEntityHint, IUse
{
	public virtual AmmoType Type => AmmoType.None;
	public virtual int AmmoCount => 30;
	protected virtual string WorldModelPath => string.Empty;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		CollisionGroup = CollisionGroup.Weapon;
		SetInteractsAs( CollisionLayer.Debris );
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

		player.GiveAmmo( Type, AmmoCount );

		Delete();
	}

	public static Ammo Create( AmmoType ammoType )
	{
		return ammoType switch
		{
			AmmoType.None => null,
			AmmoType.PistolSMG => new SMGAmmo(),
			AmmoType.Shotgun => new ShotgunAmmo(),
			AmmoType.Sniper => new SniperAmmo(),
			AmmoType.Magnum => new MagnumAmmo(),
			AmmoType.Rifle => new RifleAmmo(),
			_ => null,
		};
	}

	string IEntityHint.TextOnTick => $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to pickup {Type}";

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
		return user.IsAlive() && user is Player player;
	}
}

// public abstract partial class TTTAmmo : Prop, IEntityHint
// {
// 	/// <summary>
// 	/// The ammo type to use.
// 	/// </summary>
// 	public virtual AmmoType Type { get; set; }

// 	/// <summary>
// 	/// Amount of Ammo within Entity.
// 	/// </summary>
// 	public virtual int Amount { get; set; }

// 	/// <summary>
// 	/// Maximum amount of ammo player can carry in their inventory.
// 	/// </summary>
// 	public virtual int Max { get; set; }

// 	[Net]
// 	private int CurrentAmmo { get; set; }
// 	private int AmmoEntMax { get; set; }

// 	/// <summary>
// 	/// Fired when a player picks up any amount of ammo from the entity.
// 	/// </summary>
// 	protected Output OnPickup { get; set; }

// 	public override string ModelPath => "models/ammo/ammo_buckshot.vmdl";

// 	public override void Spawn()
// 	{
// 		base.Spawn();

// 		SetModel( ModelPath );
// 		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
// 		CollisionGroup = CollisionGroup.Weapon;
// 		SetInteractsAs( CollisionLayer.Debris );

// 		AmmoEntMax = Amount;
// 		CurrentAmmo = Amount;
// 	}

// 	public void SetCurrentAmmo( int ammo )
// 	{
// 		CurrentAmmo = ammo;
// 	}

// 	public override void TakeDamage( DamageInfo info )
// 	{
// 		PhysicsBody body = info.Body;

// 		if ( !body.IsValid() )
// 		{
// 			body = PhysicsBody;
// 		}

// 		if ( body.IsValid() && !info.Flags.HasFlag( DamageFlags.PhysicsImpact ) )
// 		{
// 			body.ApplyImpulseAt( info.Position, info.Force * 100 );
// 		}
// 	}

// 	public float HintDistance => TTTPlayer.INTERACT_DISTANCE;

// 	public string TextOnTick => $"Press {Input.GetButtonOrigin( InputButton.Use ).ToUpper()} to pickup {LibraryTitle}";

// 	public bool CanHint( TTTPlayer client )
// 	{
// 		return true;
// 	}

// 	public EntityHintPanel DisplayHint( TTTPlayer client )
// 	{
// 		return new Hint( TextOnTick );
// 	}

// 	public void Tick( TTTPlayer player )
// 	{
// 		if ( IsClient )
// 		{
// 			return;
// 		}

// 		if ( player.LifeState != LifeState.Alive )
// 		{
// 			return;
// 		}

// 		using ( Prediction.Off() )
// 		{
// 			if ( !Input.Pressed( InputButton.Use ) )
// 			{
// 				return;
// 			}

// 			bool hasWeaponOfAmmoType = false;
// 			for ( int i = 0; i < player.Inventory.Count(); ++i )
// 			{
// 				if ( player.Inventory.GetSlot( i ) is CarriableInfo weapon )
// 				{
// 					if ( weapon.Primary.AmmoType == Type )
// 					{
// 						hasWeaponOfAmmoType = true;
// 						break;
// 					}
// 				}
// 			}

// 			if ( !hasWeaponOfAmmoType )
// 			{
// 				return;
// 			}

// 			int playerAmount = player.AmmoCount( Type );

// 			if ( Max < playerAmount + Math.Ceiling( CurrentAmmo * 0.25 ) )
// 			{
// 				return;
// 			}

// 			int amountGiven = Math.Min( CurrentAmmo, Max - playerAmount );
// 			player.GiveAmmo( Type, amountGiven );
// 			CurrentAmmo -= amountGiven;
// 			OnPickup.Fire( player );

// 			if ( CurrentAmmo <= 0 || Math.Ceiling( AmmoEntMax * 0.25 ) > CurrentAmmo )
// 			{
// 				Delete();
// 			}
// 		}
// 	}
// }






































