using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/weapons/w_mg4.vmdl" )]
[Library( "ttt_weapon_huge", Title = "H.U.G.E" )]
public partial class Huge : Weapon
{
	private readonly string BulletsBodyGroup = "bullets";
	private readonly int MaxBulletsChoice = 7;

	public override void CreateViewModel()
	{
		base.CreateViewModel();

		// Start the HUGE with bullets showing.
		ViewModelEntity.SetBodyGroup( BulletsBodyGroup, MaxBulletsChoice );
	}

	public override void Simulate( Client owner )
	{
		base.Simulate( owner );

		// As we decrease ammo count, update the viewmodels "bullets" bodygroup.
		if ( IsLocalPawn )
			if ( AmmoClip <= MaxBulletsChoice )
				ViewModelEntity.SetBodyGroup( "bullets", AmmoClip );
	}
}
