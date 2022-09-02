using Sandbox;

namespace TTT;

public class GrabbableCorpse : IGrabbable
{
	public string PrimaryAttackHint => !IsHolding ? "Pickup" : AttachmentHint;
	private string AttachmentHint => !_corpse.Ropes.IsNullOrEmpty() ? "Detach" : _owner.Role.CanAttachCorpses ? "Attach" : string.Empty;
	public string SecondaryAttackHint => IsHolding ? "Drop" : string.Empty;
	public bool IsHolding => _joint.IsValid();

	private readonly Player _owner;
	private readonly Corpse _corpse;
	private PhysicsBody _handPhysicsBody;
	private readonly FixedJoint _joint;

	public GrabbableCorpse( Player player, Corpse corpse )
	{
		_owner = player;
		_corpse = corpse;

		_handPhysicsBody = new( Map.Physics );
		_handPhysicsBody.BodyType = PhysicsBodyType.Keyframed;

		var attachment = player.GetAttachment( Hands.MiddleHandsAttachment )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;

		_joint = PhysicsJoint.CreateFixed( _handPhysicsBody, _corpse.PhysicsBody );
	}

	public Entity Drop()
	{
		if ( _joint.IsValid() )
			_joint.Remove();

		_handPhysicsBody = null;
		return _corpse;
	}

	public void Update( Player player )
	{
		if ( _handPhysicsBody is null )
			return;

		foreach ( var spring in _corpse?.RopeJoints )
		{
			if ( Vector3.DistanceBetween( spring.Body1.Position, spring.Point2.LocalPosition ) > Player.UseDistance * 1.5 )
			{
				Drop();
				return;
			}
		}

		var attachment = player.GetAttachment( Hands.MiddleHandsAttachment )!.Value;
		_handPhysicsBody.Position = attachment.Position;
		_handPhysicsBody.Rotation = attachment.Rotation;
	}

	public void SecondaryAction()
	{
		if ( !_owner.Role.CanAttachCorpses )
			return;

		var trace = Trace.Ray( _owner.EyePosition, _owner.EyePosition + _owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( _owner )
			.Run();

		_corpse.RemoveRopeAttachments();

		if ( !trace.Hit || !trace.Entity.IsWorld )
			return;

		var rope = Particles.Create( "particles/rope/rope.vpcf", _corpse );
		var worldLocalPos = trace.Body.Transform.PointToLocal( trace.EndPosition );
		rope.SetPosition( 1, worldLocalPos );

		var spring = PhysicsJoint.CreateLength( _corpse.PhysicsBody, trace.Body.LocalPoint( worldLocalPos ), 10 );
		spring.SpringLinear = new( 5, 0.3f );
		spring.Collisions = true;
		spring.EnableAngularConstraint = false;

		_corpse.Ropes.Add( rope );
		_corpse.RopeJoints.Add( spring );

		Drop();
	}
}
