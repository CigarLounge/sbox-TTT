using Sandbox;
using SandboxEditor;

namespace TTT;

[EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[Library( "ttt_ammo_smg", Title = "SMG Ammo" ), HammerEntity]
public class SMGAmmo : Ammo
{
	public override AmmoType Type => AmmoType.PistolSMG;
	protected override string WorldModelPath => "models/ammo/ammo_smg/ammo_smg.vmdl";
}
