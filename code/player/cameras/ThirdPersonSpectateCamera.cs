using Sandbox;

namespace TTT;

public partial class ThirdPersonSpectateCamera : CameraMode, IObservationCamera
{
	private Vector3 DefaultPosition { get; set; }
	private const int CAMERA_DISTANCE = 120;

	private Rotation _targetRot;
	private Vector3 _targetPos;
	private Angles _lookAngles;

	public override void Activated()
	{
		base.Activated();

		if ( Local.Pawn is not Player player )
			return;

		player.UpdateObservatedPlayer();
	}

	public override void Deactivated()
	{
		if ( Local.Pawn is not Player player )
			return;

		Viewer = Local.Pawn;
		player.CurrentPlayer = null;
	}

	public override void Update()
	{
		_targetRot = Rotation.From( _lookAngles );
		Rotation = Rotation.Slerp( Rotation, _targetRot, 25f * RealTime.Delta );

		_targetPos = GetSpectatePoint() + Rotation.Forward * -CAMERA_DISTANCE;
		Position = Vector3.Lerp( Position, _targetPos, 50f * RealTime.Delta );
	}

	private Vector3 GetSpectatePoint()
	{
		if ( Local.Pawn is not Player player || !player.IsSpectatingPlayer )
			return DefaultPosition;

		return player.CurrentPlayer.EyePosition;
	}

	public override void BuildInput( InputBuilder input )
	{
		_lookAngles += input.AnalogLook;
		_lookAngles.roll = 0;

		if ( Local.Pawn is Player player )
		{
			if ( input.Pressed( InputButton.Attack1 ) )
				player.UpdateObservatedPlayer();
		}

		base.BuildInput( input );
	}
}
