using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_magnum/ammo_magnum.vmdl" )]
[Library( "ttt_ammo_magnum" )]
public partial class MagnumAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Magnum;
	public override int DefaultAmmoCount => 6;
	public override int MaxPlayerAmmo => 12;
	public override int MinAmmoToRemain => 3;
	protected override string WorldModelPath => "models/ammo/ammo_magnum/ammo_magnum.vmdl";
}
