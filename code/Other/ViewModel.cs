using Sandbox;

namespace TTT;

public class ViewModel : BaseViewModel
{
	protected float SwingInfluence = 0.05f;
	protected float ReturnSpeed = 5.0f;
	protected float MaxOffsetLength = 10.0f;
	protected float BobCycleTime = 5;
	protected Vector3 BobDirection = new( 0.0f, 1.0f, 0.3f );

	private bool _activated = false;
	private Vector3 _swingOffset;
	private float _lastPitch;
	private float _lastYaw;
	private float _bobAnim;
	private float _yawInertia;
	private float _pitchInertia;

	public override void PlaceViewmodel()
	{
		if ( !Game.LocalPawn.IsValid() )
			return;

		var inPos = Sandbox.Camera.Position;
		var inRot = Sandbox.Camera.Rotation;

		if ( !_activated )
		{
			_lastPitch = inRot.Pitch();
			_lastYaw = inRot.Yaw();

			_yawInertia = 0;
			_pitchInertia = 0;

			_activated = true;
		}

		var cameraBoneIndex = GetBoneIndex( "camera" );
		if ( cameraBoneIndex != -1 )
			inRot *= Rotation.Inverse * GetBoneTransform( cameraBoneIndex ).Rotation;

		Position = inPos;
		Rotation = inRot;

		var newPitch = Rotation.Pitch();
		var newYaw = Rotation.Yaw();

		_pitchInertia = Angles.NormalizeAngle( newPitch - _lastPitch );
		_yawInertia = Angles.NormalizeAngle( _lastYaw - newYaw );

		var playerVelocity = Game.LocalPawn.Velocity;
		var verticalDelta = playerVelocity.z * Time.Delta;
		var viewDown = Rotation.FromPitch( newPitch ).Up * -1.0f;
		verticalDelta *= 1.0f - System.MathF.Abs( viewDown.Cross( Vector3.Down ).y );
		var pitchDelta = _pitchInertia - verticalDelta * 1;
		var yawDelta = _yawInertia;

		var offset = CalcSwingOffset( pitchDelta, yawDelta );
		offset += CalcBobbingOffset( playerVelocity );

		Position += Rotation * offset;

		_lastPitch = newPitch;
		_lastYaw = newYaw;
	}

	protected Vector3 CalcSwingOffset( float pitchDelta, float yawDelta )
	{
		var swingVelocity = new Vector3( 0, yawDelta, pitchDelta );

		_swingOffset -= _swingOffset * ReturnSpeed * Time.Delta;
		_swingOffset += swingVelocity * SwingInfluence;

		if ( _swingOffset.Length > MaxOffsetLength )
			_swingOffset = _swingOffset.Normal * MaxOffsetLength;

		return _swingOffset;
	}

	protected Vector3 CalcBobbingOffset( Vector3 velocity )
	{
		_bobAnim += Time.Delta * BobCycleTime;

		var twoPI = System.MathF.PI * 2.0f;

		if ( _bobAnim > twoPI )
			_bobAnim -= twoPI;

		var speed = new Vector2( velocity.x, velocity.y ).Length;
		speed = speed > 10.0 ? speed : 0.0f;
		var offset = BobDirection * (speed * 0.005f) * System.MathF.Cos( _bobAnim );
		offset = offset.WithZ( -System.MathF.Abs( offset.z ) );

		return offset;
	}
}
