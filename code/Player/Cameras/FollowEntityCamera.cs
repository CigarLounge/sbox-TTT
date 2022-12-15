using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : BaseCamera
{
	private Entity TargetEntity { get; set; }
	private Vector3 _focusPoint = Camera.Position;

	public FollowEntityCamera( Entity entity ) => TargetEntity = entity;

	public override void BuildInput( Player player )
	{
		if ( player.IsAlive() )
			return;

		if ( TargetEntity is Corpse && Input.Down( InputButton.Jump ) )
			player.CurrentCamera = new FreeCamera();
	}

	public override void FrameSimulate( Player player )
	{
		if ( !TargetEntity.IsValid() )
			return;

		_focusPoint = Vector3.Lerp( _focusPoint, TargetEntity.Position, Time.Delta * 5.0f );

		var tr = Trace.Ray( _focusPoint, _focusPoint + player.ViewAngles.ToRotation().Forward * -130 )
			.WorldOnly()
			.Run();

		Camera.Rotation = player.ViewAngles.ToRotation();
		Camera.Position = tr.EndPosition;
		Camera.FirstPersonViewer = null;
	}

	[GameEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( Game.IsServer )
			return;

		if ( player.IsForcedSpectator )
		{
			player.CurrentCamera = new FreeCamera();
			return;
		}

		player.CurrentCamera = new FollowEntityCamera( player.Corpse );
	}
}
