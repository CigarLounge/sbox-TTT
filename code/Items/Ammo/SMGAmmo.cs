using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[Library( "ttt_ammo_smg", Title = "SMG Ammo" )]
public partial class SMGAmmo : Ammo
{
	public override AmmoType Type => AmmoType.PistolSMG;
	protected override string WorldModelPath => "models/ammo/ammo_smg/ammo_smg.vmdl";
}
