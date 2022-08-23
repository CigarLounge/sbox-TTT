using Sandbox;

namespace TTT;

public class ThirdPersonSpectateCamera : CameraMode, ISpectateCamera
{
	private const int CameraDistance = 120;
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

		player.CurrentPlayer = null;
	}

	public override void Update()
	{
		Rotation = Rotation.From( _lookAngles );

		_targetPos = Vector3.Lerp( _targetPos, GetSpectatePoint(), 50f * RealTime.Delta );

		var trace = Trace.Ray( _targetPos, _targetPos + Rotation.Forward * -CameraDistance )
			.WorldOnly()
			.Run();

		Position = trace.EndPosition;
	}

	private Vector3 GetSpectatePoint()
	{
		if ( Local.Pawn is not Player player || !player.IsSpectatingPlayer )
			return Vector3.Zero;

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
