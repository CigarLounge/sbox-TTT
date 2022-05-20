using Sandbox;
using System;

namespace TTT;

[EditorModel( "models/weapons/w_mg4.vmdl" )]
[Library( "ttt_weapon_huge", Title = "H.U.G.E" )]
public class Huge : Weapon
{
	private const string BulletsBodyGroup = "bullets";
	private const int MaxBulletsChoice = 7;

	public override void Simulate( Client client )
	{
		base.Simulate( client );

		// As we decrease ammo count, update the viewmodels "bullets" bodygroup.
		ViewModelEntity?.SetBodyGroup( BulletsBodyGroup, Math.Min( AmmoClip, MaxBulletsChoice ) );
	}
}
