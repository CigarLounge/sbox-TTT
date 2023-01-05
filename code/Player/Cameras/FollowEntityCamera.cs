using Sandbox;

namespace TTT;

public class FollowEntityCamera : CameraMode
{
	private Entity _followedEntity;
	private Vector3 _focusPoint = Camera.Position;

	public FollowEntityCamera( Entity entity )
	{
		_followedEntity = entity;

		if ( _followedEntity is Player player )
			Spectating.Player = player;

		Camera.FirstPersonViewer = null;
	}

	public override void BuildInput()
	{
		if ( !_followedEntity.IsValid() )
		{
			Current = new FreeCamera();
			return;
		}

		if ( _followedEntity is Corpse && Input.Pressed( InputButton.Jump ) )
		{
			Current = new FreeCamera();
			return;
		}

		if ( Spectating.Player.IsValid() )
		{
			if ( Input.Pressed( InputButton.Jump ) )
			{
				Current = new FirstPersonCamera( Spectating.Player );
				return;
			}

			if ( Input.Pressed( InputButton.PrimaryAttack ) )
				Spectating.FindPlayer( false );

			if ( Input.Pressed( InputButton.SecondaryAttack ) )
				Spectating.FindPlayer( true );

			_followedEntity = Spectating.Player;
		}
	}

	public override void FrameSimulate( IClient client )
	{
		if ( client.Pawn is not Player player || !_followedEntity.IsValid() )
			return;

		_focusPoint = Vector3.Lerp( _focusPoint, _followedEntity.WorldSpaceBounds.Center, Time.Delta * 5.0f );

		var tr = Trace.Ray( _focusPoint, _focusPoint + player.ViewAngles.ToRotation().Forward * -130 )
			.WorldOnly()
			.Run();

		Camera.Rotation = player.ViewAngles.ToRotation();
		Camera.Position = tr.EndPosition;
	}
}
