using Sandbox;
using System;

namespace TTT;

[Library( "ttt_grenade_discombobulator", Title = "Discombobulator" )]
[EditorModel( "models/weapons/w_frag.vmdl" )]
public class Discombobulator : Grenade
{
	private const string ExplodeSound = "discombobulator_explode-1";
	private const string Particle = "particles/discombobulator_explode.vpcf";

	protected override void OnExplode()
	{
		base.OnExplode();

		Particles.Create( Particle, Position );
		Sound.FromWorld( ExplodeSound, Position );

		float radius = 400;
		float pushForce = 1024;

		foreach ( var entity in Entity.FindInSphere( Position, radius ) )
		{
			if ( entity is not ModelEntity target || !target.IsValid() )
				continue;

			if ( !target.IsAlive() )
				continue;

			if ( !target.PhysicsBody.IsValid() )
				continue;

			if ( target.IsWorld )
				continue;

			// Ported over code from GMod, doesn't translate well to s&box
			/*		
			var dir = (targetPos - Position).Normal;
			var phys = entity.PhysicsBody;

			if ( entity is Player )
			{
				dir.z = Math.Abs( dir.z ) + 1;
				var push = dir * pushForce;
				var velocity = entity.Velocity + push;
				velocity.z = Math.Min( velocity.z, pushForce );

				if ( entity == PreviousOwner && !AllowJump )
				{
					velocity = Vector3.Random * velocity.Length;
					velocity.z = Math.Abs( velocity.z );
				}

				entity.Velocity = velocity;
			}
			else
			{
				phys.ApplyForceAt( phys.MassCenter, physForce );

			}
			*/

			var targetPos = target.PhysicsBody.MassCenter;

			if ( target is Player )
				targetPos += Vector3.Up * 63;

			var dist = Vector3.DistanceBetween( Position, targetPos );
			if ( dist > radius )
				continue;

			var trace = Trace.Ray( Position, targetPos )
				.Ignore( this )
				.WorldOnly()
				.Run();

			if ( trace.Fraction < 0.98f )
				continue;

			float distanceMul = 1.0f - Math.Clamp( dist / radius, 0.0f, 1.0f );
			float force = pushForce * distanceMul;
			var forceDir = (targetPos - Position).Normal;

			target.GroundEntity = null;
			target.Velocity += force * forceDir;
		}
	}

	static Discombobulator()
	{
		Precache.Add( Particle );
	}
}
