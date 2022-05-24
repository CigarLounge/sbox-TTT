using SandboxEditor;

namespace TTT;

[ClassName( "ttt_ammo_sniper" )]
[EditorModel( "models/ammo/ammo_sniper/ammo_sniper.vmdl" )]
[HammerEntity]
[Title( "Sniper Ammo" )]
public class SniperAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Sniper;
	public override int DefaultAmmoCount => 10;
	protected override string WorldModelPath => "models/ammo/ammo_sniper/ammo_sniper.vmdl";
}
