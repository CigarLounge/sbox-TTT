using Sandbox;

namespace TTT;

public class RagdollSpectateCamera : CameraMode, ISpectateCamera
{
	private Vector3 _focusPoint;

	public override void Activated()
	{
		base.Activated();

		_focusPoint = CurrentView.Position - GetViewOffset();

		Viewer = null;
	}

	public override void Update()
	{
		_focusPoint = Vector3.Lerp( _focusPoint, GetSpectatePoint(), Time.Delta * 5.0f );

		Rotation = Input.Rotation;

		var trace = Trace.Ray( GetSpectatePoint(), _focusPoint + GetViewOffset() )
			.WorldOnly()
			.Run();

		Position = Vector3.Lerp( Position, trace.EndPosition, 50f * RealTime.Delta );
	}

	public virtual Vector3 GetSpectatePoint()
	{
		if ( Local.Pawn is Player player && player.Corpse.IsValid() )
			return player.Corpse.Position;

		return Local.Pawn.Position;
	}

	public virtual Vector3 GetViewOffset()
	{
		return Input.Rotation.Forward * -130 + Vector3.Up * 20;
	}
}
