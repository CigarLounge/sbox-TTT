using Sandbox;

namespace TTT;

public partial class FreeCamera : BaseCamera
{
	private const int BaseMoveSpeed = 300;
	private float _moveSpeed = 1f;
	private Angles _lookAngles = Camera.Rotation.Angles();
	private Vector3 _moveInput;

	public override void BuildInput( Player player )
	{
		_moveSpeed = 1f;

		if ( Input.Down( InputButton.Run ) )
			_moveSpeed = 5f;

		if ( Input.Down( InputButton.Duck ) )
			_moveSpeed = 0.2f;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			var alivePlayer = Game.Random.FromList( Utils.GetAlivePlayers() );
			if ( alivePlayer.IsValid() )
			{
				player.CurrentCamera = new PlayerCamera( alivePlayer );
				return;
			}
		}

		if ( Input.Pressed( InputButton.Use ) )
			FindSpectateTarget( player );

		_moveInput = Input.AnalogMove;
		_lookAngles += Input.AnalogLook;
		_lookAngles.roll = 0;
	}

	public override void FrameSimulate( Player player )
	{
		var mv = _moveInput.Normal * BaseMoveSpeed * RealTime.Delta * Camera.Rotation * _moveSpeed;
		Camera.Position += mv;
		Camera.Rotation = Rotation.From( _lookAngles );
		Camera.FirstPersonViewer = null;
		PlayerCamera.Target = null;
	}

	private void FindSpectateTarget( Player player )
	{
		if ( player.HoveredEntity is Prop prop && prop.PhysicsBody is not null )
			Player.Possess( prop.NetworkIdent );
		else if ( player.HoveredEntity is Player hoveredPlayer )
			player.CurrentCamera = new PlayerCamera( hoveredPlayer );
	}
}
