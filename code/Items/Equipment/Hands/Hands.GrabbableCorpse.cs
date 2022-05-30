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

		if ( !trace.Hit || !trace.Entity.IsWorld )
		{
			_corpse.ClearAttachments();
			return;
		}

		var rope = Particles.Create( "particles/rope.vpcf", _corpse );
		var worldLocalPos = trace.Body.Transform.PointToLocal( trace.EndPosition );
		rope.SetPosition( 1, worldLocalPos );

		var spring = PhysicsJoint.CreateLength( _corpse.PhysicsBody, trace.Body.LocalPoint( worldLocalPos ), 10 );
		spring.SpringLinear = new( 5, 0.3f );
		spring.Collisions = true;
		spring.EnableAngularConstraint = false;

		_corpse.Ropes.Add( rope );
		_corpse.RopeSprings.Add( spring );

		Drop();
	}
}
