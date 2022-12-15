using Editor;

namespace TTT;

[Category( "Ammo" )]
[ClassName( "ttt_ammo_magnum" )]
[EditorModel( "models/ammo/ammo_magnum/ammo_magnum.vmdl" )]
[HammerEntity]
[Title( "Magnum Ammo" )]
public class MagnumAmmo : Ammo
{
	protected override AmmoType Type => AmmoType.Magnum;
	protected override int DefaultAmmoCount => 6;
	protected override string WorldModelPath => "models/ammo/ammo_magnum/ammo_magnum.vmdl";
}
