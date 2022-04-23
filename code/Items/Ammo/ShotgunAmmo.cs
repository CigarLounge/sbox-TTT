using Sandbox;

namespace TTT;

[Library( "ttt_ammo_shotgun", Title = "Shotgun Ammo" )]
[Hammer.EditorModel( "models/ammo/ammo_shotgun/ammo_shotgun.vmdl" )]
public partial class ShotgunAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Shotgun;
	public override int DefaultAmmoCount => 5;
	protected override string WorldModelPath => "models/ammo/ammo_shotgun/ammo_shotgun.vmdl";
}
