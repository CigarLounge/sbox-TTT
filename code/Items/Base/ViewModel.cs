using System;
using Sandbox;

namespace TTT;

partial class ViewModel : BaseViewModel
{
	private float _animSpeed;
	private Vector3 _targetVectorPos;
	private Vector3 _targetVectorRot;
	private float _targetFOV;
	private Vector3 _finalVectorPos;
	private Vector3 _finalVectorRot;
	private float _finalFOV = 65f;
	private Rotation _lastEyeRot;
	private float _jumpTime;
	private float _landTime;
	private Vector3 _localVel;

	public override void PostCameraSetup( ref CameraSetup camSetup )
	{
		base.PostCameraSetup( ref camSetup );
		camSetup.ViewModel.FieldOfView = 65f;
		Rotation = camSetup.Rotation;
		Position = camSetup.Position;

		_finalVectorPos = _finalVectorPos.LerpTo( _targetVectorPos, _animSpeed * RealTime.Delta );
		_finalVectorRot = _finalVectorRot.LerpTo( _targetVectorRot, _animSpeed * RealTime.Delta );
		_finalFOV = MathX.LerpTo( _finalFOV, _targetFOV, _animSpeed * RealTime.Delta );
		_animSpeed = 8;

		Rotation *= Rotation.From( _finalVectorRot.x, _finalVectorRot.y, _finalVectorRot.z );
		Position += _finalVectorPos.z * Rotation.Up + _finalVectorPos.y * Rotation.Forward + _finalVectorPos.x * Rotation.Right;
		camSetup.ViewModel.FieldOfView = _finalFOV;

		_localVel = new Vector3( Owner.Rotation.Right.Dot( Owner.Velocity ), Owner.Rotation.Forward.Dot( Owner.Velocity ), Owner.Velocity.z );

		_targetVectorPos = new Vector3();
		_targetVectorRot = new Vector3();
		_targetFOV = 65f;

		HandleIdleAnimation();
		HandleWalkAnimation();
		HandleSwayAnimation();
		HandleJumpAnimation();
	}

	private void HandleIdleAnimation()
	{
		float breatheTime = RealTime.Now * 2.0f;
		_targetVectorPos -= new Vector3( MathF.Cos( breatheTime / 4.0f ) / 8.0f, 0.0f, -MathF.Cos( breatheTime / 4.0f ) / 32.0f );
		_targetVectorRot -= new Vector3( MathF.Cos( breatheTime / 5.0f ), MathF.Cos( breatheTime / 4.0f ), MathF.Cos( breatheTime / 7.0f ) );

		if ( Input.Down( InputButton.Run ) )
			_targetVectorPos += new Vector3( -1.0f, -1.0f, 0.5f );
	}

	private void HandleWalkAnimation()
	{
		float breatheTime = RealTime.Now * 16.0f;
		float walkSpeed = new Vector3( Owner.Velocity.x, Owner.Velocity.y, 0.0f ).Length;
		float maxWalkSpeed = 200.0f;
		float roll = 0.0f;
		float yaw = 0.0f;

		if ( Owner.GroundEntity == null )
			return;

		if ( _localVel.x < 0.0f )
			yaw = 3.0f * (_localVel.x / maxWalkSpeed);

		_targetVectorPos -= new Vector3( (-MathF.Cos( breatheTime / 2.0f ) / 5.0f) * walkSpeed / maxWalkSpeed - yaw / 4.0f, 0.0f, 0.0f );
		_targetVectorRot -= new Vector3( (Math.Clamp( MathF.Cos( breatheTime ), -0.3f, 0.3f ) * 2.0f) * walkSpeed / maxWalkSpeed, (-MathF.Cos( breatheTime / 2.0f ) * 1.2f) * walkSpeed / maxWalkSpeed - yaw * 1.5f, roll );
	}

	private void HandleSwayAnimation()
	{
		int swayspeed = 5;

		_lastEyeRot = Rotation.Lerp( _lastEyeRot, Owner.EyeRotation, swayspeed * RealTime.Delta );

		Angles angDif = Owner.EyeRotation.Angles() - _lastEyeRot.Angles();
		angDif = new Angles( angDif.pitch, MathX.RadianToDegree( MathF.Atan2( MathF.Sin( MathX.DegreeToRadian( angDif.yaw ) ), MathF.Cos( MathX.DegreeToRadian( angDif.yaw ) ) ) ), 0 );

		_targetVectorPos += new Vector3( Math.Clamp( angDif.yaw * 0.04f, -1.5f, 1.5f ), 0.0f, Math.Clamp( angDif.pitch * 0.04f, -1.5f, 1.5f ) );
		_targetVectorRot += new Vector3( Math.Clamp( angDif.pitch * 0.2f, -4.0f, 4.0f ), Math.Clamp( angDif.yaw * 0.2f, -4.0f, 4.0f ), 0.0f );
	}

	private void HandleJumpAnimation()
	{
		if ( Owner.GroundEntity == null )
			_landTime = RealTime.Now + 0.31f;

		if ( _landTime < RealTime.Now && _landTime != 0.0f )
		{
			_landTime = 0.0f;
			_jumpTime = 0.0f;
		}

		if ( Input.Down( InputButton.Jump ) && _jumpTime == 0.0f )
		{
			_jumpTime = RealTime.Now + 0.31f;
			_landTime = 0.0f;
		}

		if ( _jumpTime > RealTime.Now )
		{
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
			float breatheTime = RealTime.Now * 30.0f;
			_targetVectorPos += new Vector3( MathF.Cos( breatheTime / 2.0f ) / 16.0f, 0.0f, -5.0f + (MathF.Sin( breatheTime / 3.0f ) / 16.0f) ) / 4.0f;
			_targetVectorRot += new Vector3( 10.0f - (MathF.Sin( breatheTime / 3.0f ) / 4.0f), MathF.Cos( breatheTime / 2.0f ) / 4.0f, -5.0f ) / 4.0f;
			_animSpeed = 20.0f;
		}
		else if ( _landTime > RealTime.Now )
		{
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
