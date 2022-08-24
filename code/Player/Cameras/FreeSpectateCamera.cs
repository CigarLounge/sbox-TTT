using Sandbox;

namespace TTT;

public class FreeSpectateCamera : CameraMode, ISpectateCamera
{
	private Angles _lookAngles;
	private Vector3 _moveInput;
	private float _moveSpeed;
	private Vector3 _targetPos;
	private Rotation _targetRot;

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

	public override void Update()
	{
		var mv = _moveInput.Normal * 300 * RealTime.Delta * Rotation * _moveSpeed;

		_targetRot = Rotation.From( _lookAngles );
		_targetPos += mv;

		Position = _targetPos;
		Rotation = _targetRot;
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
			FindSpectateTarget();

		base.BuildInput( input );
	}

	private void FindSpectateTarget()
	{
		var trace = Trace.Ray( Position, Position + Rotation.Forward * Player.UseDistance )
			.Run();

		if ( trace.Entity is Prop prop && prop.PhysicsBody is not null )
			Player.Possess( prop.NetworkIdent );
		else if ( trace.Entity is Player player )
			Player.SpectatePlayer( player.NetworkIdent );
	}
}
