using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_hegrenade", Title = "HE Grenade" )]
public class HEGrenade : Throwable<HEGrenadeEntity>
{
}

[Hammer.Skip]
[Library( "ttt_entity_hegrenade", Title = "HE Grenade" )]
public class HEGrenadeEntity : BaseGrenade
{
	protected override void Explode()
	{
		base.Explode();

		Game.Explosion( this, Owner, Position, 400, 200, 20.0f );
	}
}
