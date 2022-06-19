using SandboxEditor;

namespace TTT;

[Category( "Ammo" )]
[ClassName( "ttt_ammo_rifle" )]
[EditorModel( "models/ammo/ammo_rifle/ammo_rifle.vmdl" )]
[HammerEntity]
[Title( "Rifle Ammo" )]
public class RifleAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Rifle;
	protected override string WorldModelPath => "models/ammo/ammo_rifle/ammo_rifle.vmdl";
}
