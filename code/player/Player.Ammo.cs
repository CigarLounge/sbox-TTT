using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	[Net]
	public List<int> Ammo { get; set; } = new();

	public int[] AmmoCap = new int[] { 0, 60, 16, 20, 12, 40 };

	public void ClearAmmo()
	{
		Ammo.Clear();
	}

	public int AmmoCount( AmmoType type )
	{
		int iType = (int)type;

		if ( Ammo == null )
			return 0;

		if ( Ammo.Count <= iType )
			return 0;

		return Ammo[(int)type];
	}

	public bool SetAmmo( AmmoType type, int amount )
	{
		int iType = (int)type;

		if ( !Host.IsServer )
			return false;

		if ( Ammo == null )
			return false;

		while ( Ammo.Count <= iType )
		{
			Ammo.Add( 0 );
		}

		Ammo[(int)type] = amount;

		return true;
	}

	public bool GiveAll( int amount )
	{
		if ( !Host.IsServer )
			return false;

		if ( Ammo == null )
			return false;

		foreach ( AmmoType ammoType in Enum.GetValues( typeof( AmmoType ) ) )
		{
			GiveAmmo( ammoType, amount );
		}

		return true;
	}

	public int GiveAmmo( AmmoType type, int amount )
	{
		if ( !Host.IsServer || Ammo == null )
			return 0;

		var ammoPickedUp = Math.Min( amount, AmmoCap[(int)type] - AmmoCount( type ) );
		if ( ammoPickedUp > 0 )
		{
			SetAmmo( type, AmmoCount( type ) + ammoPickedUp );
			PlaySound( RawStrings.AmmoPickupSound );
		}

		return ammoPickedUp;
	}

	public int TakeAmmo( AmmoType type, int amount )
	{
		if ( Ammo == null )
			return 0;

		var available = AmmoCount( type );
		amount = Math.Min( available, amount );

		SetAmmo( type, available - amount );

		return amount;
	}
}
