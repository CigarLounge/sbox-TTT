using System;

using Sandbox;

namespace TTT;

partial class ViewModel : BaseViewModel
{
	private float _animSpeed;

	// Target animation values
	private Vector3 _targetVectorPos;
	private Vector3 _targetVectorRot;
	private float _targetFOV;

	// Finalized animation values
	private Vector3 _finalVectorPos;
	private Vector3 _finalVectorRot;
	private float _finalFOV = 65f;

	// Sway
	private Rotation _lastEyeRot;

	// Jumping Animation
	private float _jumpTime;
	private float _landTime;

	// Helpful values
	private Vector3 _localVel;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );
		FieldOfView = 65f;
		Rotation = camSetup.Rotation;
		Position = camSetup.Position;

		// Smoothly transition the vectors with the target values
		_finalVectorPos = _finalVectorPos.LerpTo( _targetVectorPos, _animSpeed * RealTime.Delta );
		_finalVectorRot = _finalVectorRot.LerpTo( _targetVectorRot, _animSpeed * RealTime.Delta );
		_finalFOV = MathX.LerpTo( _finalFOV, _targetFOV, _animSpeed * RealTime.Delta );
		_animSpeed = 8;

		// Change the angles and positions of the viewmodel with the new vectors
		Rotation *= Rotation.From( _finalVectorRot.x, _finalVectorRot.y, _finalVectorRot.z );
		Position += _finalVectorPos.z * Rotation.Up + _finalVectorPos.y * Rotation.Forward + _finalVectorPos.x * Rotation.Right;
		FieldOfView = _finalFOV;

		// I'm sure there's something already that does this for me, but I spend an hour
		// searching through the wiki and a bunch of other garbage and couldn't find anything...
		// So I'm doing it manually. Problem solved.
		_localVel = new Vector3( Owner.Rotation.Right.Dot( Owner.Velocity ), Owner.Rotation.Forward.Dot( Owner.Velocity ), Owner.Velocity.z );

		// Initialize the target vectors for this frame
		_targetVectorPos = new Vector3();
		_targetVectorRot = new Vector3();
		_targetFOV = 65f;

		// Handle different animations
		HandleIdleAnimation();
		HandleWalkAnimation();
		HandleSwayAnimation();
		HandleJumpAnimation();
	}

	private void HandleIdleAnimation()
	{
		// Perform a "breathing" animation
		float breatheTime = RealTime.Now * 2.0f;
		_targetVectorPos -= new Vector3( MathF.Cos( breatheTime / 4.0f ) / 8.0f, 0.0f, -MathF.Cos( breatheTime / 4.0f ) / 32.0f );
		_targetVectorRot -= new Vector3( MathF.Cos( breatheTime / 5.0f ), MathF.Cos( breatheTime / 4.0f ), MathF.Cos( breatheTime / 7.0f ) );

		// Crouching animation
		if ( Input.Down( InputButton.Duck ) )
			_targetVectorPos += new Vector3( -1.0f, -1.0f, 0.5f );
	}

	private void HandleWalkAnimation()
	{
		float breatheTime = RealTime.Now * 16.0f;
		float walkSpeed = new Vector3( Owner.Velocity.x, Owner.Velocity.y, 0.0f ).Length;
		float maxWalkSpeed = 200.0f;
		float roll = 0.0f;
		float yaw = 0.0f;

		// Check if on the ground
		if ( Owner.GroundEntity == null )
			return;

		// Check if sprinting

		// Check for sideways velocity to sway the gun slightly
		if ( _localVel.x < 0.0f )
			yaw = 3.0f * (_localVel.x / maxWalkSpeed);

		// Perform walk cycle
		_targetVectorPos -= new Vector3( (-MathF.Cos( breatheTime / 2.0f ) / 5.0f) * walkSpeed / maxWalkSpeed - yaw / 4.0f, 0.0f, 0.0f );
		_targetVectorRot -= new Vector3( (Math.Clamp( MathF.Cos( breatheTime ), -0.3f, 0.3f ) * 2.0f) * walkSpeed / maxWalkSpeed, (-MathF.Cos( breatheTime / 2.0f ) * 1.2f) * walkSpeed / maxWalkSpeed - yaw * 1.5f, roll );
	}

	private void HandleSwayAnimation()
	{
		int swayspeed = 5;

		// Lerp the eye position
		_lastEyeRot = Rotation.Lerp( _lastEyeRot, Owner.EyeRotation, swayspeed * RealTime.Delta );

		// Calculate the difference between our current eye angles and old (lerped) eye angles
		Angles angDif = Owner.EyeRotation.Angles() - _lastEyeRot.Angles();
		angDif = new Angles( angDif.pitch, MathX.RadianToDegree( MathF.Atan2( MathF.Sin( MathX.DegreeToRadian( angDif.yaw ) ), MathF.Cos( MathX.DegreeToRadian( angDif.yaw ) ) ) ), 0 );

		// Perform sway
		_targetVectorPos += new Vector3( Math.Clamp( angDif.yaw * 0.04f, -1.5f, 1.5f ), 0.0f, Math.Clamp( angDif.pitch * 0.04f, -1.5f, 1.5f ) );
		_targetVectorRot += new Vector3( Math.Clamp( angDif.pitch * 0.2f, -4.0f, 4.0f ), Math.Clamp( angDif.yaw * 0.2f, -4.0f, 4.0f ), 0.0f );
	}

	private void HandleJumpAnimation()
	{
		// If we're not on the ground, reset the landing animation time
		if ( Owner.GroundEntity == null )
			_landTime = RealTime.Now + 0.31f;

		// Reset the timers once they elapse
		if ( _landTime < RealTime.Now && _landTime != 0.0f )
		{
			_landTime = 0.0f;
			_jumpTime = 0.0f;
		}

		// If we jumped, start the animation
		if ( Input.Down( InputButton.Jump ) && _jumpTime == 0.0f )
		{
			_jumpTime = RealTime.Now + 0.31f;
			_landTime = 0.0f;
		}

		if ( _jumpTime > RealTime.Now )
		{
			// If we jumped, do a curve upwards
			float f = 0.31f - (_jumpTime - RealTime.Now);
			float xx = BezierY( f, 0.0f, -4.0f, 0.0f );
			float yy = 0.0f;
			float zz = BezierY( f, 0.0f, -2.0f, -5.0f );
			float pt = BezierY( f, 0.0f, -4.36f, 10.0f );
			float yw = xx;
			float rl = BezierY( f, 0.0f, -10.82f, -5.0f );
			_targetVectorPos += new Vector3( xx, yy, zz ) / 4.0f;
			_targetVectorRot += new Vector3( pt, yw, rl ) / 4.0f;
			_animSpeed = 20.0f;
		}
		else if ( Owner.GroundEntity == null )
		{
			// Shaking while falling
			float breatheTime = RealTime.Now * 30.0f;
			_targetVectorPos += new Vector3( MathF.Cos( breatheTime / 2.0f ) / 16.0f, 0.0f, -5.0f + (MathF.Sin( breatheTime / 3.0f ) / 16.0f) ) / 4.0f;
			_targetVectorRot += new Vector3( 10.0f - (MathF.Sin( breatheTime / 3.0f ) / 4.0f), MathF.Cos( breatheTime / 2.0f ) / 4.0f, -5.0f ) / 4.0f;
			_animSpeed = 20.0f;
		}
		else if ( _landTime > RealTime.Now )
		{
			// If we landed, do a fancy curve downwards
			float f = _landTime - RealTime.Now;
			float xx = BezierY( f, 0.0f, -4.0f, 0.0f );
			float yy = 0.0f;
			float zz = BezierY( f, 0.0f, -2.0f, -5.0f );
			float pt = BezierY( f, 0.0f, -4.36f, 10.0f );
			float yw = xx;
			float rl = BezierY( f, 0.0f, -10.82f, -5.0f );
			_targetVectorPos += new Vector3( xx, yy, zz ) / 2.0f;
			_targetVectorRot += new Vector3( pt, yw, rl ) / 2.0f;
			_animSpeed = 20.0f;
		}
	}

	private static float BezierY( float f, float a, float b, float c )
	{
		f *= 3.2258f;
		return MathF.Pow( 1.0f - f, 2.0f ) * a + 2.0f * (1.0f - f) * f * b + MathF.Pow( f, 2.0f ) * c;
	}
}
