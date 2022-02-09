using Sandbox;

namespace TTT.Items
{
	[Library( "ttt_ammo_rpg", Title = "RPG Ammo" )]
	[Hammer.EditorModel( "weapons/swb/explosives/rpg-7/swb_w_rpg7_rocket_he.vmdl" )]
	partial class RPGAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.RPG;
		public override int Amount => 1;
		public override int Max => 1;
		public override string ModelPath => "weapons/swb/explosives/rpg-7/swb_w_rpg7_rocket_he.vmdl";
	}
}
