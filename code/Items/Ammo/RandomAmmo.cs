using Sandbox;

namespace TTT;

[Hammer.EntityTool( "Random Ammo", "TTT", "Place where a random ammo box will spawn in the beginning of the round." )]
[Hammer.EditorModel( "models/ammo/ammo_smg/ammo_smg.vmdl" )]
[Library( "ttt_random_ammo" )]
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
