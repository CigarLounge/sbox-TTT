using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_shotgun/ammo_shotgun.vmdl" )]
[Library( "ttt_ammo_shotgun" )]
public partial class ShotgunAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Shotgun;
	public override int MaxAmmoCount => 5;
	protected override string WorldModelPath => "models/ammo/ammo_shotgun/ammo_shotgun.vmdl";
}
