using Sandbox;

namespace TTT;

public class GrabbableCorpse : IGrabbable
{
	private readonly Player _owner;
	private readonly Corpse _corpse;
	private PhysicsBody _handPhysicsBody;
	private readonly PhysicsBody _corpsePhysicsBody;
	private readonly int _corpseBone;
	private FixedJoint _joint;

	public bool IsHolding
	{
		get => _joint.IsValid();
	}

	public GrabbableCorpse( Player player, Corpse corpse, PhysicsBody physicsBodyCorpse, int corpseBone )
	{
		_owner = player;
		_corpse = corpse;
		_corpsePhysicsBody = physicsBodyCorpse;
		_corpseBone = corpseBone;

		// TODO MATT
		_handPhysicsBody = Map.Physics.Body;
		_handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;

		Transform attachment = player.GetAttachment( Hands.MIDDLE_HANDS_ATTACHMENT )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;

		// TODO Matt figure out this breaking change.
		// _joint = _handPhysicsBody.En
		// 	.From( _handPhysicsBody )
		// 	.To( physicsBodyCorpse )
		// 	.Create();
	}

	public void Drop()
	{
		if ( _joint.IsValid() )
		{
			_joint.Remove();
		}

		_handPhysicsBody = null;
	}

	public void Update( Player player )
	{
		if ( _handPhysicsBody == null )
		{
			return;
		}

		// If the player grabs the corpse while it is attached with a rope, we should automatically
		// drop it if they walk away far enough.
		foreach ( PhysicsJoint spring in _corpse?.RopeSprings )
		{
			// TODO MATT
			// if ( Vector3.DistanceBetween( spring.Body1.Position, spring.Anchor2 ) > Hands.MAX_INTERACT_DISTANCE )
			// {
			// 	Drop();

			// 	return;
			// }
		}

		Transform attachment = player.GetAttachment( Hands.MIDDLE_HANDS_ATTACHMENT )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;
	}

	public void SecondaryAction()
	{
		TraceResult tr = Trace.Ray( _owner.EyePosition, _owner.EyePosition + _owner.EyeRotation.Forward * Hands.MAX_INTERACT_DISTANCE )
			.Ignore( _owner )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() )
		{
			_corpse.ClearAttachments();

			return;
		}

		// TODO MATT
		Entity attachEnt = tr.Body.IsValid() ? tr.Body.GetEntity() : tr.Entity;

		if ( !attachEnt.IsWorld )
		{
			_corpse.ClearAttachments();

			return;
		}

		// TODO MATT
		// Particles rope = Particles.Create( "particles/rope.vpcf" );
		// rope.SetEntityBone( 0, _corpsePhysicsBody.Entity, _corpseBone, new Transform( _corpsePhysicsBody.Transform.PointToLocal( _corpsePhysicsBody.Position ) * (1.0f / _corpsePhysicsBody.Entity.Scale) ) );
		// rope.SetPosition( 1, tr.Body.Transform.PointToLocal( tr.EndPos ) );

		// SpringJoint spring = PhysicsJoint.Spring
		// 		.From( _corpsePhysicsBody, _corpsePhysicsBody.Transform.PointToLocal( _corpsePhysicsBody.Position ) )
		// 		.To( tr.Body, tr.Body.Transform.PointToLocal( tr.EndPos ) )
		// 		.WithFrequency( 1f )
		// 		.WithDampingRatio( 1f )
		// 		.WithReferenceMass( _corpsePhysicsBody.PhysicsGroup.Mass )
		// 		.WithMinRestLength( 0 )
		// 		.WithMaxRestLength( 10f )
		// 		.Create();

		// _corpse.Ropes.Add( rope );
		// _corpse.RopeSprings.Add( spring );

		Drop();
	}
}
