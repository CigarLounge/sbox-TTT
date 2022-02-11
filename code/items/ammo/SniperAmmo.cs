using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_sniper", Title = "Sniper Ammo" )]
	[Spawnable]
	[Hammer.EditorModel( "models/ammo/ammo_sniper/ammo_sniper.vmdl" )]
	partial class SniperAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Sniper;
		public override int Amount => 5;
		public override int Max => 36;
		public override string ModelPath => "models/ammo/ammo_sniper/ammo_sniper.vmdl";
	}
}
