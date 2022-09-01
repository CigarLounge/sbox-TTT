using Sandbox;

namespace TTT;

public class FreeSpectateCamera : CameraMode, ISpectateCamera
{
	private Player _owner;
	private Angles _lookAngles;
	private Vector3 _moveInput;
	private float _moveSpeed;
	private Vector3 _targetPos;
	private Rotation _targetRot;

	public override void Activated()
	{
		_owner = Entity as Player;

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
		if ( _owner.HoveredEntity is Prop prop && prop.PhysicsBody is not null )
			Player.Possess( prop.NetworkIdent );
		else if ( _owner.HoveredEntity is Player player )
			_owner.Camera = new ThirdPersonSpectateCamera { InitialSpectatedPlayer = player };
	}
}
