using SandboxEditor;

namespace TTT;

[Category( "Ammo" )]
[ClassName( "ttt_ammo_smg" )]
[EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[HammerEntity]
[Title( "SMG Ammo" )]
public class SMGAmmo : Ammo
{
	protected override AmmoType Type => AmmoType.PistolSMG;
	protected override string WorldModelPath => "models/ammo/ammo_smg/ammo_smg.vmdl";
}
