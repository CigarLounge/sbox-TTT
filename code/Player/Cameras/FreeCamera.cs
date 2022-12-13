using Sandbox;

namespace TTT;

public partial class FreeCamera : BaseCamera
{
	private const int BaseMoveSpeed = 300;
	private float _moveSpeed = 1f;
	private Angles _lookAngles;
	private Vector3 _moveInput;

	public override void BuildInput()
	{
		_moveSpeed = 1f;

		if ( Input.Down( InputButton.Run ) )
			_moveSpeed = 5f;

		if ( Input.Down( InputButton.Duck ) )
			_moveSpeed = 0.2f;

		_moveInput = Input.AnalogMove;
		_lookAngles += Input.AnalogLook;
		_lookAngles.roll = 0;
	}

	public override void FrameSimulate( Player player )
	{
		var mv = _moveInput.Normal * BaseMoveSpeed * RealTime.Delta * Camera.Rotation * _moveSpeed;
		Camera.Position += mv;
		Camera.Rotation = Rotation.From( _lookAngles );
		Camera.FirstPersonViewer = null;
	}
}