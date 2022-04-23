using Sandbox;
using System;

namespace TTT;

[Library( "ttt_weapon_huge", Title = "H.U.G.E" )]
[Hammer.EditorModel( "models/weapons/w_mg4.vmdl" )]
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
