using Sandbox;

namespace TTT.Items;

[Library( "ttt_ammo_shotgun", Title = "Shotgun Ammo" )]
[Hammer.EditorModel( "models/ammo/ammo_shotgun/ammo_shotgun.vmdl" )]
partial class ShotgunAmmo : TTTAmmo
{
	public override AmmoType Type => AmmoType.Shotgun;
	public override int Amount => 8;
	public override int Max => 36;
	public override string ModelPath => "models/ammo/ammo_shotgun/ammo_shotgun.vmdl";
}
