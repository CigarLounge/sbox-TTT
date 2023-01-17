using Sandbox;
using Sandbox.Physics;

namespace TTT;

public static class ModelEntityExtensions
{
	public static PhysicsJoint GmodWeld( this ModelEntity a, ModelEntity b, int bBone )
	{
		var aP = new PhysicsPoint( a.PhysicsBody );
		bBone = int.Clamp( bBone, 0, 1 );
		var bTransform = b.GetBoneTransform( bBone );

		var bP = new PhysicsPoint( b.PhysicsBody, b.Transform.ToLocal( bTransform ).Position, b.Rotation.Inverse * a.Rotation );
		var weld = PhysicsJoint.CreateFixed( aP, bP );
		weld.SpringLinear = new PhysicsSpring
		{
			Damping = 5f,
			Frequency = 104.5f,
		};
		weld.SpringAngular = new PhysicsSpring
		{
			Damping = 10f,
			Frequency = 150f,
		};
		weld.Strength = 350000f;
		return weld;
	}


	public static bool IsStoodOnByPlayer( this ModelEntity ent )
	{
		if ( !ent.IsValid() )
			return false;

		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn is not Player player || !player.IsAlive )
				continue;

			if ( player.GroundEntity == ent )
				return true;
		}

		return false;
	}
}
