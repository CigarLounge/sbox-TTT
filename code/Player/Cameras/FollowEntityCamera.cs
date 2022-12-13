using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : BaseCamera
{
	[Net, Local]
	private Entity TargetEntity { get; set; }
	private Vector3 _focusPoint = Camera.Position;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity entity ) => TargetEntity = entity;

	public override void Simulate( Player player )
	{
		if ( !TargetEntity.IsValid() )
			player.CurrentCamera = new FreeCamera();
	}

	public override void FrameSimulate( Player player )
	{
		if ( !TargetEntity.IsValid() )
			return;

		_focusPoint = Vector3.Lerp( _focusPoint, TargetEntity.Position, Time.Delta * 5.0f );

		var tr = Trace.Ray( _focusPoint, _focusPoint + (player.ViewAngles.ToRotation().Forward * -130 + Vector3.Up * 20) )
			.WorldOnly()
			.Run();

		Camera.Rotation = player.ViewAngles.ToRotation();
		Camera.Position = tr.EndPosition;
		Camera.FirstPersonViewer = null;
	}
}
