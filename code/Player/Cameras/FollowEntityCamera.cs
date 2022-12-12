using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : BaseCamera
{
	[Net, Local]
	public Entity TargetEntity { get; private set; }

	private Vector3 _focusPoint;

	public FollowEntityCamera() { }

	public FollowEntityCamera( Entity entity )
	{
		TargetEntity = entity;
	}

	public override void BuildInput() { }

	public override void FrameSimulate()
	{
		_focusPoint = Vector3.Lerp( _focusPoint, TargetEntity.PhysicsGroup.MassCenter, Time.Delta * 5.0f );

		var tr = Trace.Ray( _focusPoint + Vector3.Up * 12, _focusPoint )
			.WorldOnly()
			.Ignore( TargetEntity )
			.Radius( 6 )
			.Run();

		Camera.Position = tr.EndPosition;
		Camera.FirstPersonViewer = null;
	}

	public virtual Vector3 GetViewOffset()
	{
		if ( Game.LocalPawn is not Player player ) return Vector3.Zero;

		return player.EyeRotation.Forward * (-130 * 1) + Vector3.Up * (20 * 1);
	}
}
