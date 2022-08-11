using Sandbox;

namespace TTT;

public class FreeSpectateCamera : CameraMode, ISpectateCamera
{
	private Angles _lookAngles;
	private Vector3 _moveInput;

	private Vector3 _targetPos;
	private Rotation _targetRot;

	private float _moveSpeed;

	private const float LerpMode = 0;

	public override void Activated()
	{
		base.Activated();

		_targetPos = CurrentView.Position;
		_targetRot = CurrentView.Rotation;

		Position = _targetPos;
		Rotation = _targetRot;
		_lookAngles = Rotation.Angles();

		Viewer = Local.Pawn;
	}

	public override void Deactivated()
	{
		base.Deactivated();

		Viewer = null;
	}

	public override void Update()
	{
		var mv = _moveInput.Normal * 300 * RealTime.Delta * Rotation * _moveSpeed;

		_targetRot = Rotation.From( _lookAngles );
		_targetPos += mv;

		Position = Vector3.Lerp( Position, _targetPos, 10 * RealTime.Delta * (1 - LerpMode) );
		Rotation = Rotation.Slerp( Rotation, _targetRot, 10 * RealTime.Delta * (1 - LerpMode) );
	}

	public override void BuildInput( InputBuilder input )
	{
		_moveInput = input.AnalogMove;

		_moveSpeed = 1;

		if ( input.Down( InputButton.Run ) )
			_moveSpeed = 5;

		if ( input.Down( InputButton.Duck ) )
			_moveSpeed = 0.2f;

		_lookAngles += input.AnalogLook;
		_lookAngles.roll = 0;

		if ( input.Pressed( InputButton.Use ) )
			FindTargetProp();

		base.BuildInput( input );
	}

	private void FindTargetProp()
	{
		var trace = Trace.Ray( Position, Position + Rotation.Forward * Player.UseDistance )
			.Run();

		if ( trace.Entity is Prop prop && prop.PhysicsBody is not null )
			Player.Possess( prop.NetworkIdent );
	}
}
