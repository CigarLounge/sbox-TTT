using Sandbox;

namespace TTT;

[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[Library( "ttt_ammo_random", Title = "Random Ammo" )]
public class RandomAmmo : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Never;

		var ent = Ammo.Create( (AmmoType)Rand.Int( 1, 5 ) );
		ent.Position = Position;
		ent.Rotation = Rotation;
	}
}
