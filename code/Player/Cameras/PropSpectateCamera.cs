using Sandbox;

namespace TTT;

public class PropSpectateCamera : CameraMode, ISpectateCamera
{
	private Vector3 _focusPoint;
	private Prop _prop => (Local.Pawn as Player).PossessedProp;

	public override void Activated()
	{
		base.Activated();

		_focusPoint = CurrentView.Position - GetViewOffset();

		Position = CurrentView.Position;
		Rotation = CurrentView.Rotation;

		Viewer = null;
	}

	public override void Update()
	{
		if ( !_prop.IsValid() )
			return;

		_focusPoint = Vector3.Lerp( _focusPoint, _prop.Position, Time.Delta * 5.0f );

		Rotation = Input.Rotation;

		var trace = Trace.Ray( _prop.Position, _focusPoint + GetViewOffset() )
			.WorldOnly()
			.Run();

		Position = Vector3.Lerp( Position, trace.EndPosition, 50f * RealTime.Delta );
	}

	public virtual Vector3 GetViewOffset()
	{
		return Input.Rotation.Forward * -130 + Vector3.Up * 20;
	}
}
