using System;
using Editor;
using Sandbox;

namespace TTT;

[Category( "Weapons" )]
[ClassName( "ttt_weapon_huge" )]
[EditorModel( "models/weapons/w_mg4.vmdl" )]
[HammerEntity]
[Title( "H.U.G.E" )]
public class Huge : Weapon
{
	private const string BulletsBodyGroup = "bullets";
	private const int MaxBulletsChoice = 7;

	public override void Simulate( IClient client )
	{
		base.Simulate( client );

		// As we decrease ammo count, update the viewmodels "bullets" bodygroup.
		ViewModelEntity?.SetBodyGroup( BulletsBodyGroup, Math.Min( AmmoClip, MaxBulletsChoice ) );
	}

	public override void CreateViewModel()
	{
		base.CreateViewModel();

		ViewModelEntity.SetBodyGroup( BulletsBodyGroup, Math.Min( AmmoClip, MaxBulletsChoice ) );
	}
}
