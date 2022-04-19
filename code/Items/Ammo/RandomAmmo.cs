using Sandbox;

namespace TTT;

[Library( "ttt_ammo_random" )]
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
