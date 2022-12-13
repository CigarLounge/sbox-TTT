using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : BaseCamera
{
	[Net, Local]
	public Entity TargetEntity { get; private set; }

	private Vector3 _focusPoint = Camera.Position;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity entity ) => TargetEntity = entity;

	public override void BuildInput() { }

	public override void FrameSimulate( Player player )
	{
		_focusPoint = Vector3.Lerp( _focusPoint, TargetEntity.Position, Time.Delta * 5.0f );

		var tr = Trace.Ray( _focusPoint, _focusPoint + (player.ViewAngles.ToRotation().Forward * -130 + Vector3.Up * 20) )
			.WorldOnly()
			.Run();

		Camera.Rotation = player.ViewAngles.ToRotation();
		Camera.Position = tr.EndPosition;
		Camera.FirstPersonViewer = null;
	}
}
