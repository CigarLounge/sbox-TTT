using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	[Net]
	public List<int> Ammo { get; set; } = new();

	private static readonly int[] AmmoCap = new int[] { 0, 60, 16, 20, 12, 60 };

	public void ClearAmmo()
	{
		Ammo.Clear();
	}

	public int AmmoCount( AmmoType type )
	{
		int iType = (int)type;

		if ( Ammo is null )
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

		if ( Ammo is null )
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

		if ( Ammo is null )
			return false;

		foreach ( AmmoType ammoType in Enum.GetValues( typeof( AmmoType ) ) )
		{
			GiveAmmo( ammoType, amount );
		}

		return true;
	}

	public int GiveAmmo( AmmoType type, int amount )
	{
		if ( !Host.IsServer || Ammo is null )
			return 0;

		int ammoPickedUp = Math.Min( amount, AmmoCap[(int)type] - AmmoCount( type ) );
		if ( ammoPickedUp > 0 )
		{
			using ( Prediction.Off() )
			{
				SetAmmo( type, AmmoCount( type ) + ammoPickedUp );
				PlaySound( Strings.AmmoPickupSound );
			}
		}

		return ammoPickedUp;
	}

	public int TakeAmmo( AmmoType type, int amount )
	{
		if ( Ammo is null )
			return 0;

		int available = AmmoCount( type );
		amount = Math.Min( available, amount );

		SetAmmo( type, available - amount );

		return amount;
	}
}
