using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_magnum", Title = "Magnum Ammo" )]
	[Spawnable]
	[Hammer.EditorModel( "models/ammo/ammo_magnum/ammo_magnum.vmdl" )]
	partial class MagnumAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Revolver;
		public override int Amount => 12;
		public override int Max => 60;
		public override string ModelPath => "models/ammo/ammo_magnum/ammo_magnum.vmdl";
	}
}
