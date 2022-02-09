using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_rifle", Title = "Rifle Ammo" )]
	[Spawnable]
	[Hammer.EditorModel( "models/ammo/ammo_rifle/ammo_rifle.vmdl" )]
	partial class RifleAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Rifle;
		public override int Amount => 30;
		public override int Max => 90;
		public override string ModelPath => "models/ammo/ammo_rifle/ammo_rifle.vmdl";
	}
}
