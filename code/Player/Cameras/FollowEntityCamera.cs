using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : CameraMode, ISpectateCamera
{
	[Net, Local]
	public Entity FollowedEntity { get; private set; }

	private Player _owner;
	private Vector3 _focusPoint;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity followedEntity ) => FollowedEntity = followedEntity;

	public override void Activated()
	{
		base.Activated();

		_owner = Entity as Player;
		_focusPoint = CurrentView.Position - GetViewOffset();

		Position = CurrentView.Position;
		Rotation = CurrentView.Rotation;
	}

	public override void Update()
	{
		if ( !FollowedEntity.IsValid() )
			return;

		Rotation = _owner.ViewAngles.ToRotation();

		_focusPoint = Vector3.Lerp( _focusPoint, FollowedEntity.Position, 50f * RealTime.Delta );

		var trace = Trace.Ray( _focusPoint, _focusPoint + GetViewOffset() )
			.WorldOnly()
			.Run();

		Position = trace.EndPosition;
	}

	public virtual Vector3 GetViewOffset()
	{
		return _owner.ViewAngles.ToRotation().Forward * -130 + Vector3.Up * 20;
	}
}
