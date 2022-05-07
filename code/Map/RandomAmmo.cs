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

		var entity = Ammo.Create( (AmmoType)Rand.Int( 1, 5 ) );
		entity.Position = Position;
		entity.Rotation = Rotation;
	}
}
