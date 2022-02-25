using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_sniper/ammo_sniper.vmdl" )]
[Library( "ttt_ammo_sniper" )]
public partial class SniperAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Sniper;
	public override int MaxAmmoCount => 5;
	protected override string WorldModelPath => "models/ammo/ammo_sniper/ammo_sniper.vmdl";
}
