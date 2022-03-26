using Sandbox;

namespace TTT;

[Hammer.Skip]
[Library( "ttt_equipment_magnetostick", Title = "Magneto Stick" )]
public partial class MagnetoStick : Carriable
{
	[Net]
	public bool BeamActive { get; set; }

	[Net]
	public Entity GrabbedEntity { get; set; }

	[Net]
	public int GrabbedBone { get; set; }

	[Net]
	public Vector3 GrabbedPos { get; set; }

	private const float MIN_TARGET_DISTANCE = 0.0f;
	private const float MAX_TARGET_DISTANCE = Player.USE_DISTANCE;
	private const float LINEAR_FREQUENCY = 20.0f;
	private const float LINEAR_DAMPING_RATIO = 1.0f;
	private const float ANGULAR_FREQUENCY = 20.0f;
	private const float ANGULAR_DAMPING_RADIO = 1.0f;

	private PhysicsBody _holdBody;
	private PhysicsBody _velBody;
	private FixedJoint _holdJoint;
	private FixedJoint _velJoint;

	private PhysicsBody _heldBody;
	private Vector3 _heldPos;
	private Rotation _heldRot;

	private float _holdDistance;
	private bool _isGrabbing;

	private PhysicsBody HeldBody => _heldBody;

	public override void Simulate( Client client )
	{
		if ( Owner is not Player owner ) return;

		var eyePos = owner.EyePosition;
		var eyeDir = owner.EyeRotation.Forward;
		var eyeRot = Rotation.From( new Angles( 0.0f, owner.EyeRotation.Angles().yaw, 0.0f ) );

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			(Owner as AnimEntity)?.SetAnimParameter( "b_attack", true );

			if ( !_isGrabbing )
				_isGrabbing = true;
		}

		bool grabEnabled = _isGrabbing && Input.Down( InputButton.Attack1 );

		BeamActive = grabEnabled;

		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				if ( !_holdBody.IsValid() )
					return;

				if ( grabEnabled )
				{
					if ( _heldBody.IsValid() )
					{
						UpdateGrab( eyePos, eyeRot, eyeDir );
					}
					else
					{
						TryStartGrab( owner, eyePos, eyeRot, eyeDir );
					}
				}
				else if ( _isGrabbing )
				{
					GrabEnd();
				}
			}
		}
	}

	private void TryStartGrab( Player owner, Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
	{
		var tr = Trace.Ray( eyePos, eyePos + eyeDir * MAX_TARGET_DISTANCE )
			.UseHitboxes()
			.Ignore( owner, false )
			.HitLayer( CollisionLayer.Debris )
			.Run();

		if ( !tr.Hit || !tr.Entity.IsValid() || tr.Entity.IsWorld || tr.StartedSolid ) return;

		var rootEnt = tr.Entity.Root;
		var body = tr.Body;

		if ( !body.IsValid() || tr.Entity.Parent.IsValid() )
		{
			if ( rootEnt.IsValid() && rootEnt.PhysicsGroup != null )
			{
				body = (rootEnt.PhysicsGroup.BodyCount > 0 ? rootEnt.PhysicsGroup.GetBody( 0 ) : null);
			}
		}

		if ( !body.IsValid() )
			return;

		//
		// Don't move keyframed, unless it's a player
		//
		if ( body.BodyType == PhysicsBodyType.Keyframed && rootEnt is not Player )
			return;

		GrabInit( body, eyePos, tr.EndPosition, eyeRot );

		GrabbedEntity = rootEnt;
		GrabbedPos = body.Transform.PointToLocal( tr.EndPosition );
		GrabbedBone = body.GroupIndex;

		Client?.Pvs.Add( GrabbedEntity );
	}

	private void UpdateGrab( Vector3 eyePos, Rotation eyeRot, Vector3 eyeDir )
	{
		GrabMove( eyePos, eyeDir, eyeRot );
	}

	private void Activate()
	{
		if ( !IsServer )
			return;

		if ( !_holdBody.IsValid() )
		{
			_holdBody = new PhysicsBody( Map.Physics )
			{
				BodyType = PhysicsBodyType.Keyframed
			};
		}

		if ( !_velBody.IsValid() )
		{
			_velBody = new PhysicsBody( Map.Physics )
			{
				BodyType = PhysicsBodyType.Dynamic,
				AutoSleep = false
			};
		}
	}

	private void Deactivate()
	{
		if ( IsServer )
		{
			GrabEnd();

			_holdBody?.Remove();
			_holdBody = null;

			_velBody?.Remove();
			_velBody = null;
		}
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		Activate();
	}

	public override void ActiveEnd( Entity ent, bool dropped )
	{
		base.ActiveEnd( ent, dropped );

		Deactivate();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		Deactivate();
	}

	public override void OnCarryDrop( Entity dropper )
	{
	}

	private void GrabInit( PhysicsBody body, Vector3 startPos, Vector3 grabPos, Rotation rot )
	{
		if ( !body.IsValid() )
			return;

		GrabEnd();

		_isGrabbing = true;
		_heldBody = body;
		_holdDistance = Vector3.DistanceBetween( startPos, grabPos );
		_holdDistance = _holdDistance.Clamp( MIN_TARGET_DISTANCE, MAX_TARGET_DISTANCE );

		_heldRot = rot.Inverse * _heldBody.Rotation;

		_holdBody.Position = grabPos;
		_holdBody.Rotation = _heldBody.Rotation;

		_velBody.Position = grabPos;
		_velBody.Rotation = _heldBody.Rotation;

		_heldBody.Sleeping = false;
		_heldBody.AutoSleep = false;

		_holdJoint = PhysicsJoint.CreateFixed( _holdBody, _heldBody.WorldPoint( grabPos ) );
		_holdJoint.SpringLinear = new PhysicsSpring( LINEAR_FREQUENCY, LINEAR_DAMPING_RATIO );
		_holdJoint.SpringAngular = new PhysicsSpring( ANGULAR_FREQUENCY, ANGULAR_DAMPING_RADIO );

		_velJoint = PhysicsJoint.CreateFixed( _holdBody, _velBody );
		_velJoint.SpringLinear = new PhysicsSpring( LINEAR_FREQUENCY, LINEAR_DAMPING_RATIO );
		_velJoint.SpringAngular = new PhysicsSpring( ANGULAR_FREQUENCY, ANGULAR_DAMPING_RADIO );

	}

	private void GrabEnd()
	{
		_holdJoint?.Remove();
		_holdJoint = null;

		_velJoint?.Remove();
		_velJoint = null;

		if ( _heldBody.IsValid() )
		{
			_heldBody.AutoSleep = true;
		}

		Client?.Pvs.Remove( GrabbedEntity );

		_heldBody = null;
		GrabbedEntity = null;
		_isGrabbing = false;
	}

	private void GrabMove( Vector3 startPos, Vector3 dir, Rotation rot )
	{
		if ( !_heldBody.IsValid() )
			return;

		_holdBody.Position = startPos + dir * _holdDistance;

		if ( GrabbedEntity is Player player )
		{
			player.Velocity = _velBody.Velocity;
			player.Position = _holdBody.Position - _heldPos;

			var controller = player.GetActiveController();
			if ( controller != null )
			{
				controller.Velocity = _velBody.Velocity;
			}

			return;
		}

		_holdBody.Rotation = rot * _heldRot;
	}

	private void MoveTargetDistance( float distance )
	{
		_holdDistance += distance;
		_holdDistance = _holdDistance.Clamp( MIN_TARGET_DISTANCE, MAX_TARGET_DISTANCE );
	}

	protected virtual void DoRotate( Rotation eye, Vector3 input )
	{
		var localRot = eye;
		localRot *= Rotation.FromAxis( Vector3.Up, input.x );
		localRot *= Rotation.FromAxis( Vector3.Right, input.y );
		localRot = eye.Inverse * localRot;

		_heldRot = localRot * _heldRot;
	}

	public override void BuildInput( InputBuilder owner )
	{
		if ( !GrabbedEntity.IsValid() )
			return;

		if ( !owner.Down( InputButton.Attack1 ) )
			return;
	}

	public bool IsUsable( Entity user ) => Owner == null || HeldBody.IsValid();
}
