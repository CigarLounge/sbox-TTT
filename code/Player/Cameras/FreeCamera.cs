using Sandbox;

namespace TTT;

public class FreeCamera : CameraMode
{
	private const int BaseMoveSpeed = 300;
	private float _moveSpeed = 1f;
	private Angles _lookAngles = Camera.Rotation.Angles();
	private Vector3 _moveInput;

	public FreeCamera()
	{
		Spectating.Player = null;
		Camera.FirstPersonViewer = null;
	}

	public override void BuildInput()
	{
		_moveSpeed = 1f;

		if ( Input.Down( InputButton.Run ) )
			_moveSpeed = 5f;

		if ( Input.Down( InputButton.Duck ) )
			_moveSpeed = 0.2f;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			var alivePlayer = Game.Random.FromList( Utils.GetPlayersWhere( p => p.IsAlive ) );
			if ( alivePlayer.IsValid() )
				Current = new FollowEntityCamera( alivePlayer );
		}

		if ( Input.Pressed( InputButton.Use ) )
			FindSpectateTarget( (Player)Game.LocalPawn );

		_moveInput = Input.AnalogMove;
		_lookAngles += Input.AnalogLook;
	}

	public override void FrameSimulate( IClient client )
	{
		var mv = _moveInput.Normal * BaseMoveSpeed * RealTime.Delta * Camera.Rotation * _moveSpeed;

		if ( Camera.Rotation.Roll() > 90f || Camera.Rotation.Roll() < -90f )
			_lookAngles.pitch = _lookAngles.pitch.Clamp( -90f, 90f );

		Camera.Position += mv;
		Camera.Rotation = Rotation.From( _lookAngles );
	}

	private void FindSpectateTarget( Player player )
	{
		if ( player.HoveredEntity is Prop prop && prop.PhysicsBody is not null )
			Player.Possess( prop.NetworkIdent );
		else if ( player.HoveredEntity is Player hoveredPlayer )
			Current = new FirstPersonCamera( hoveredPlayer );
	}
}
