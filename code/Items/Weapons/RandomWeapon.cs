using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

[Library( "ttt_weapon_random" )]
public class RandomWeapon : Entity
{
	/// <summary>
	/// Cached weapons list to use when `ExcludedWeapons` is NOT provided.
	/// </summary>
	private static readonly List<Type> _cachedWeaponTypes = new();

	private const int WEAPON_DISTANCE_UP = 4;


	[Property( "Ammo Type to Spawn", "If changed, will only spawn weapons of the selected ammo type." )]
	public AmmoType SelectedAmmoType { get; set; } = AmmoType.None;

	/// <summary>
	/// Defines the amount of matching ammo entities that should be spawned near the weapons.
	/// </summary>
	[Property( Title = "Amount of Ammo" )]
	public int AmmoToSpawn { get; set; } = 0;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;

		if ( _cachedWeaponTypes.IsNullOrEmpty() )
		{
			var weapons = Library.GetAll<Weapon>();
			foreach ( var weaponType in weapons )
			{
				var weaponInfo = Asset.GetInfo<WeaponInfo>( Library.GetAttribute( weaponType ).Name );
				if ( weaponInfo is not null && weaponInfo.Spawnable )
					_cachedWeaponTypes.Add( weaponType );
			}
		}

		if ( SelectedAmmoType == AmmoType.None )
		{
			Activate( _cachedWeaponTypes );
		}
		else
		{
			var selectedAmmoWeaponTypes = new List<Type>();
			foreach ( var type in _cachedWeaponTypes )
			{
				var weaponInfo = Asset.GetInfo<WeaponInfo>( Library.GetAttribute( type ).Name );
				if ( weaponInfo is not null && weaponInfo.AmmoType == SelectedAmmoType )
					selectedAmmoWeaponTypes.Add( type );
			}

			Activate( selectedAmmoWeaponTypes );
		}
	}

	public void Activate( List<Type> weaponTypes )
	{
		if ( weaponTypes.Count <= 0 )
			return;

		var weapon = Library.Create<Weapon>( Rand.FromList( weaponTypes ) );
		if ( weapon is null )
			return;

		weapon.Position = Position + (Vector3.Up * WEAPON_DISTANCE_UP);
		weapon.Rotation = Rotation;

		if ( weapon.Info.AmmoType == AmmoType.None )
			return;

		for ( int i = 0; i < AmmoToSpawn; ++i )
		{
			var ammo = Ammo.Create( weapon.Info.AmmoType );
			ammo.Position = Position;
			ammo.Rotation = Rotation;
		}
	}
}
