using Sandbox;

namespace TTT;

[EditorModel( "models/ammo/ammo_rifle/ammo_rifle.vmdl" )]
[Library( "ttt_ammo_rifle", Title = "Rifle Ammo" )]
public class RifleAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Rifle;
	protected override string WorldModelPath => "models/ammo/ammo_rifle/ammo_rifle.vmdl";
}
