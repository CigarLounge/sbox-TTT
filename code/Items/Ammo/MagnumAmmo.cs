﻿using Sandbox;

namespace TTT.Items;

[Hammer.EditorModel( "models/ammo/ammo_magnum/ammo_magnum.vmdl" )]
[Library( "ttt_ammo_magnum", Title = "Magnum Ammo" )]
public class MagnumAmmo : Ammo
{
	public override AmmoType Type => AmmoType.Magnum;
	public override int DefaultAmmoCount => 6;
	protected override string WorldModelPath => "models/ammo/ammo_magnum/ammo_magnum.vmdl";
}
