using Sandbox;
using SandboxEditor;

namespace TTT;

[EditorModel( "models/ammo/ammo_shotgun/ammo_shotgun.vmdl" )]
[Library( "ttt_ammo_shotgun", Title = "Shotgun Ammo" ), HammerEntity]
public class ShotgunAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Shotgun;
	public override int DefaultAmmoCount => 5;
	protected override string WorldModelPath => "models/ammo/ammo_shotgun/ammo_shotgun.vmdl";
}
