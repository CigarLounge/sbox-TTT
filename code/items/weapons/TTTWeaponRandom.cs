using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT.Items;

[Hammer.EntityTool( "Random Weapon", "TTT", "TODO" )]
[Library( "ttt_weapon_random" )]
public class TTTWeaponRandom : Entity
{
	private static readonly int AMMO_DISTANCE_UP = 24;

	[Property( "Weapons to Spawn", "Comma seperated list of weapons that can potentially spawn here. If empty all [Spawnable] weapons will be conisdered. Ex. \"ttt_weapon_m9, ttt_weapon_m4\"" )]
	public string WeaponsToSpawn { get; private set; } = "";

	/// <summary>
	/// Defines the amount of matching ammo entities that should be spawned near the weapons.
	/// </summary>
	[Property( Title = "Amount of Ammo" )]
	public int AmmoToSpawn { get; set; } = 0;

	public void Activate()
	{
		List<Type> wepTypes = new();

		if ( string.IsNullOrEmpty( WeaponsToSpawn ) )
		{
			wepTypes = Utils.GetTypesWithAttribute<SWB_Base.WeaponBase, SpawnableAttribute>();
		}
		else
		{
			// No string array support in hammer, let's do a little trimming.
			var weaponNames = WeaponsToSpawn.Split( ',' ).Select( ( w ) => w.Trim() );
			foreach ( var name in weaponNames )
			{
				var weaponType = Utils.GetTypeByLibraryTitle<Type>( name );
				if ( weaponType != null ) wepTypes.Add( weaponType );
			}
		}

		if ( wepTypes.Count <= 0 )
		{
			return;
		}

		Type weaponTypeToSpawn = Utils.RNG.FromList( wepTypes );
		SWB_Base.WeaponBase weapon = Utils.GetObjectByType<SWB_Base.WeaponBase>( weaponTypeToSpawn );
		weapon.Position = Position;
		weapon.Rotation = Rotation;
		weapon.Spawn();

		if ( weapon is not ICarriableItem carriable || carriable.DroppedType == null )
		{
			return; // If the choosen weapon doesn't use ammo we don't need to spawn any.
		}

		if ( !carriable.DroppedType.IsSubclassOf( typeof( TTTAmmo ) ) )
		{
			Log.Error( $"The defined ammo type {carriable.DroppedType.Name} for the weapon {carriable.GetItemData().Title} is not a descendant of {typeof( TTTAmmo ).Name}." );
			return;
		}

		for ( int i = 0; i < AmmoToSpawn; i++ )
		{
			TTTAmmo ammo = Utils.GetObjectByType<TTTAmmo>( carriable.DroppedType );
			ammo.Position = weapon.Position + Vector3.Up * AMMO_DISTANCE_UP;
			ammo.Spawn();
		}
	}
}
