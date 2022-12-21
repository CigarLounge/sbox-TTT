using Sandbox;

namespace TTT;

public partial class FollowEntityCamera : CameraMode
{
	private Entity FollowedEntity { get; set; }
	private Vector3 _focusPoint = Camera.Position;
	private readonly bool _isFollowingPlayer;

	public FollowEntityCamera( Entity entity )
	{
		FollowedEntity = entity;

		_isFollowingPlayer = FollowedEntity is Player;
		if ( _isFollowingPlayer )
			Target = FollowedEntity as Player;
	}

	public override void BuildInput( Player player )
	{
		if ( !FollowedEntity.IsValid() )
			player.CurrentCamera = new FreeCamera();

		if ( player.IsAlive() )
			return;

		if ( FollowedEntity is Corpse && Input.Pressed( InputButton.Jump ) )
			player.CurrentCamera = new FreeCamera();

		if ( _isFollowingPlayer )
		{
			if ( Input.Pressed( InputButton.Jump ) )
				player.CurrentCamera = new FirstPersonCamera( Target );

			if ( Input.Pressed( InputButton.PrimaryAttack ) )
				SwapSpectatedPlayer( false );

			if ( Input.Pressed( InputButton.SecondaryAttack ) )
				SwapSpectatedPlayer( true );

			FollowedEntity = Target;
		}
	}

	public override void FrameSimulate( Player player )
	{
		if ( !FollowedEntity.IsValid() )
			return;

		_focusPoint = Vector3.Lerp( _focusPoint, FollowedEntity.WorldSpaceBounds.Center, Time.Delta * 5.0f );

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
