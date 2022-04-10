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

		_handPhysicsBody = new( Map.Physics );
		_handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;

		var attachment = player.GetAttachment( Hands.MIDDLE_HANDS_ATTACHMENT )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;

		_joint = PhysicsJoint.CreateFixed( _handPhysicsBody, physicsBodyCorpse );
	}

	public void Drop()
	{
		if ( _joint.IsValid() )
			_joint.Remove();

		_handPhysicsBody = null;
	}

	public void Update( Player player )
	{
		if ( _handPhysicsBody is null )
			return;

		// If the player grabs the corpse while it is attached with a rope, we should automatically
		// drop it if they walk away far enough.
		// TODO: Matt.
		foreach ( var spring in _corpse?.RopeSprings )
		{
			// if ( Vector3.DistanceBetween( spring.Body1.Position, spring ) > Hands.MAX_INTERACT_DISTANCE )
			// {
			// 	Drop();
			// 	return;
			// }
		}

		var attachment = player.GetAttachment( Hands.MIDDLE_HANDS_ATTACHMENT )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;
	}

	public void SecondaryAction()
	{
		if ( _owner.Team != Team.Traitors )
			return;

		var trace = Trace.Ray( _owner.EyePosition, _owner.EyePosition + _owner.EyeRotation.Forward * Hands.MAX_INTERACT_DISTANCE )
			.Ignore( _owner )
			.Run();

		if ( !trace.Hit || !trace.Entity.IsValid() )
		{
			_corpse.ClearAttachments();
			return;
		}

		var attachEnt = trace.Body.IsValid() ? trace.Body.GetEntity() : trace.Entity;
		if ( !attachEnt.IsWorld )
		{
			_corpse.ClearAttachments();
			return;
		}

		var rope = Particles.Create( "particles/rope.vpcf" );
		rope.SetEntityBone( 0, _corpsePhysicsBody.GetEntity(), _corpseBone, new Transform( _corpsePhysicsBody.Transform.PointToLocal( _corpsePhysicsBody.Position ) * (1.0f / _corpsePhysicsBody.GetEntity().Scale) ) );
		rope.SetPosition( 1, trace.Body.Transform.PointToLocal( trace.EndPosition ) );

		var from = new PhysicsPoint( _corpsePhysicsBody, _corpsePhysicsBody.Transform.PointToLocal( _corpsePhysicsBody.Position ) );
		var to = new PhysicsPoint( trace.Body, trace.Body.Transform.PointToLocal( trace.EndPosition ) );
		var spring = PhysicsJoint.CreateSpring( from, to, 0f, 10f );

		_corpse.Ropes.Add( rope );
		_corpse.RopeSprings.Add( spring );

		Drop();
	}
}
