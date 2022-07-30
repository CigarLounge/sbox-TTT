using Sandbox;

namespace TTT;

public class ThirdPersonSpectateCamera : CameraMode, ISpectateCamera
{
	private Vector3 DefaultPosition { get; }
	private const int CameraDistance = 120;

	private Rotation _targetRot;
	private Vector3 _targetPos;
	private Angles _lookAngles;

	public override void Activated()
	{
		base.Activated();

		if ( Local.Pawn is not Player player )
			return;

		player.UpdateSpectatedPlayer();
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

		_targetPos = GetSpectatePoint() + Rotation.Forward * -CameraDistance;

		var trace = Trace.Ray( GetSpectatePoint(), _targetPos )
			.WorldOnly()
			.Run();

		Position = Vector3.Lerp( Position, trace.EndPosition, 50f * RealTime.Delta );
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
			if ( input.Pressed( InputButton.PrimaryAttack ) )
				player.UpdateSpectatedPlayer( 1 );
			else if ( input.Pressed( InputButton.SecondaryAttack ) )
				player.UpdateSpectatedPlayer( -1 );
		}

		base.BuildInput( input );
	}
}
