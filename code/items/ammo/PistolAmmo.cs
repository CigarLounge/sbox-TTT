using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_pistol", Title = "Pistol Ammo" )]
	[Spawnable]
	[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
	partial class PistolAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Pistol;
		public override int Amount => 12;
		public override int Max => 60;
		public override string ModelPath => "models/ammo/ammo_smg/ammo_smg.vmdl";
	}
}
