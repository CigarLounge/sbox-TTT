using Sandbox;

namespace TTT;

public class GrabbableCorpse : IGrabbable
{
	private readonly Player _owner;
	private readonly Corpse _corpse;
	private PhysicsBody _handPhysicsBody;
	private readonly int _corpseBone;
	private readonly FixedJoint _joint;

	public bool IsHolding
	{
		get => _joint.IsValid();
	}

	public GrabbableCorpse( Player player, Corpse corpse, int corpseBone )
	{
		_owner = player;
		_corpse = corpse;
		_corpseBone = corpseBone;

		_handPhysicsBody = new( Map.Physics );
		_handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;

		var attachment = player.GetAttachment( Hands.MiddleHandsAttachment )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;

		_joint = PhysicsJoint.CreateFixed( _handPhysicsBody, _corpse.PhysicsBody );
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
		// drop it if they walk away far enough. We need to bug FacePunch about giving us access
		// about the set point and the current position of the body.
		// TODO: Matt.

		var attachment = player.GetAttachment( Hands.MiddleHandsAttachment )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;
	}

	public void SecondaryAction()
	{
		if ( _owner.Team != Team.Traitors )
			return;

		var trace = Trace.Ray( _owner.EyePosition, _owner.EyePosition + _owner.EyeRotation.Forward * Player.UseDistance )
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
		rope.SetEntityBone( 0, _corpse.PhysicsBody.GetEntity(), _corpseBone, new Transform( _corpse.PhysicsBody.Transform.PointToLocal( _corpse.PhysicsBody.Position ) * (1.0f / _corpse.PhysicsBody.GetEntity().Scale) ) );
		rope.SetPosition( 1, trace.Body.Transform.PointToLocal( trace.EndPosition ) );

		var from = new PhysicsPoint( _corpse.PhysicsBody, _corpse.PhysicsBody.Transform.PointToLocal( _corpse.PhysicsBody.Position ) );
		var to = new PhysicsPoint( trace.Body, trace.Body.Transform.PointToLocal( trace.EndPosition ) );
		var spring = PhysicsJoint.CreateSpring( from, to, 0f, 10f );

		_corpse.Ropes.Add( rope );
		_corpse.RopeSprings.Add( spring );

		Drop();
	}
}
