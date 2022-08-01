using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : CameraMode, ISpectateCamera
{
	[Net, Local] public Entity FollowedEntity { get; set; }
	private Vector3 _focusPoint;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity followedEntity ) { FollowedEntity = followedEntity; }

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

		_focusPoint = Vector3.Lerp( _focusPoint, FollowedEntity.Position, Time.Delta * 5.0f );

		Rotation = Input.Rotation;

		var trace = Trace.Ray( FollowedEntity.Position, _focusPoint + GetViewOffset() )
			.WorldOnly()
			.Run();

		Position = Vector3.Lerp( Position, trace.EndPosition, 50f * RealTime.Delta );
	}

	public virtual Vector3 GetViewOffset()
	{
		return Input.Rotation.Forward * -130 + Vector3.Up * 20;
	}
}
