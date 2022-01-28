using Sandbox;

namespace TTT.Items
{
	[Library( "Revolver Ammo" )]
	[Spawnable]
	[Hammer.EditorModel( "models/ammo/ammo_9mm.vmdl" )]
	partial class RevolverAmmo : TTTAmmo
	{
		public override SWB_Base.AmmoType AmmoType => SWB_Base.AmmoType.Revolver;
		public override int Amount => 12;
		public override int Max => 60;
		public override string ModelPath => "models/ammo/ammo_9mm.vmdl";
	}
}
