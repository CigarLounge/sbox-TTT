using Sandbox;
using SandboxEditor;
using System;
using System.Collections.Generic;

namespace TTT;

[ClassName( "ttt_weapon_random" )]
[EditorModel( "models/weapons/w_mp5.vmdl" )]
[HammerEntity]
[Title( "Random Weapon" )]
public class RandomWeapon : Entity
{
	/// <summary>
	/// Cached weapons list to use when `ExcludedWeapons` is NOT provided.
	/// </summary>
	private static readonly List<Type> _cachedWeaponTypes = new();
	private const int WEAPON_DISTANCE_UP = 4;

	[Description( "If changed, will only spawn weapons of the selected ammo type." )]
	[Property]
	public AmmoType SelectedAmmoType { get; set; } = AmmoType.None;

	[Description( "Defines the amount of matching ammo entities that should be spawned near the weapons" )]
	[Title( "Amount of ammo" )]
	[Property]
	public int AmmoToSpawn { get; set; } = 0;

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;
		CacheWeaponTypes();

		var weaponTypes = _cachedWeaponTypes;
		if ( SelectedAmmoType != AmmoType.None )
		{
			weaponTypes = new List<Type>();
			foreach ( var type in _cachedWeaponTypes )
			{
				var weaponInfo = GameResource.GetInfo<WeaponInfo>( type );
				if ( weaponInfo is not null && weaponInfo.AmmoType == SelectedAmmoType )
					weaponTypes.Add( type );
			}
		}

		if ( weaponTypes.IsNullOrEmpty() )
			return;

		var weapon = TypeLibrary.Create<Weapon>( Rand.FromList( weaponTypes ) );
		if ( weapon is null )
			return;

		weapon.Position = Position + (Vector3.Up * WEAPON_DISTANCE_UP);
		weapon.Rotation = Rotation;

		if ( weapon.Info.AmmoType == AmmoType.None )
			return;

		for ( var i = 0; i < AmmoToSpawn; ++i )
		{
			var ammo = Ammo.Create( weapon.Info.AmmoType );
			ammo.Position = Position;
			ammo.Rotation = Rotation;
		}
	}

	private void CacheWeaponTypes()
	{
		if ( !_cachedWeaponTypes.IsNullOrEmpty() )
			return;

		var weapons = TypeLibrary.GetTypes<Weapon>();
		foreach ( var weaponType in weapons )
		{
			var weaponInfo = GameResource.GetInfo<WeaponInfo>( weaponType );
			if ( weaponInfo is not null && weaponInfo.Spawnable )
				_cachedWeaponTypes.Add( weaponType );
		}
	}
}
