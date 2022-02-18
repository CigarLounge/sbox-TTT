using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_smg", Title = "SMG/Pistol Ammo", Spawnable = true )]
	[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
	partial class SMGAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.SMG;
		public override int Amount => 30;
		public override int Max => 90;
		public override string ModelPath => "models/ammo/ammo_smg/ammo_smg.vmdl";
	}
}
