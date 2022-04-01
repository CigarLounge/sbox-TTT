using Sandbox;
using System;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_discombobulator", Title = "Discombobulator" )]
public class Discombobulator : Throwable<DiscombobulatorEntity>
{
}

[Hammer.Skip]
[Library( "ttt_entity_discombobulator", Title = "Discombobulator" )]
public class DiscombobulatorEntity : BaseGrenade
{
	protected override void Explode()
	{
		base.Explode();

		PlaySound( RawStrings.DiscombobulatorExplodeSound );

		var overlaps = Entity.FindInSphere( Position, 800 );

		foreach ( var overlap in overlaps )
		{
			if ( overlap is not ModelEntity entity || !entity.IsValid() )
				continue;

			if ( !entity.IsAlive() )
				continue;

			if ( !entity.PhysicsBody.IsValid() )
				continue;

			if ( entity.IsWorld )
				continue;

			var targetPos = entity.PhysicsBody.MassCenter;

			var dist = Vector3.DistanceBetween( Position, targetPos );
			if ( dist > 400 )
				continue;

			var trace = Trace.Ray( Position, targetPos )
				.Ignore( this )
				.WorldOnly()
				.Run();

			if ( trace.Fraction < 0.98f )
				continue;

			float distanceMul = 1.0f - Math.Clamp( dist / 800, 0.0f, 1.0f );
			float force = 2 * distanceMul * entity.PhysicsBody.Mass;
			var forceDir = (targetPos - Position).Normal;

			entity.ApplyAbsoluteImpulse( force * forceDir );
		}
	}
}
