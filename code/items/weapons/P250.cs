using Sandbox;

namespace TTT;

[Hammer.EditorModel( "weapons/rust_pistol/rust_pistol.vmdl" )]
[Library( "ttt_weapon_p250", Title = "P250", Spawnable = true )]
public partial class P250 : Weapon
{
	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetParam( "holdtype", 1 );
		anim.SetParam( "aimat_weight", 1.0f );
		anim.SetParam( "holdtype_handedness", 0 );
	}
}
