using Sandbox;

namespace TTT.Items;

[Library( "ttt_ammo_magnum", Title = "Magnum Ammo" )]
[Hammer.EditorModel( "models/ammo/ammo_magnum/ammo_magnum.vmdl" )]
partial class MagnumAmmo : TTTAmmo
{
	public override AmmoType Type => AmmoType.Magnum;
	public override int Amount => 12;
	public override int Max => 60;
	public override string ModelPath => "models/ammo/ammo_magnum/ammo_magnum.vmdl";
}
