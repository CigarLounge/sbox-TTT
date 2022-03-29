using Sandbox;

namespace TTT;

public class RagdollSpectateCamera : CameraMode, ISpectateCamera
{
	private Vector3 FocusPoint;

	public override void Activated()
	{
		base.Activated();

		FocusPoint = CurrentView.Position - GetViewOffset();

		Viewer = null;
	}

	public override void Update()
	{
		FocusPoint = Vector3.Lerp( FocusPoint, GetSpectatePoint(), Time.Delta * 5.0f );

		Rotation = Input.Rotation;

		var trace = Trace.Ray( GetSpectatePoint(), FocusPoint + GetViewOffset() )
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
