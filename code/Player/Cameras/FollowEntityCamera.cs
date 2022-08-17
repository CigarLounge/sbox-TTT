using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : CameraMode, ISpectateCamera
{
	[Net, Local]
	public Entity FollowedEntity { get; private set; }

	private Vector3 _focusPoint;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity followedEntity ) => FollowedEntity = followedEntity;

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
		if ( !FollowedEntity.IsValid() )
			return;

		Rotation = Input.Rotation;

		_focusPoint = Vector3.Lerp( _focusPoint, FollowedEntity.Position, 50f * RealTime.Delta );

		var trace = Trace.Ray( _focusPoint, _focusPoint + GetViewOffset() )
			.WorldOnly()
			.Run();

		Position = trace.EndPosition;
	}

	public virtual Vector3 GetViewOffset()
	{
		return Input.Rotation.Forward * -130 + Vector3.Up * 20;
	}
}
