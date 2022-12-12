using System.Linq;
using Sandbox;

namespace TTT;

public class SpectatorCamera : PlayerCamera
{
	public bool IsFree { get; set; } = false;

	protected virtual float BaseMoveSpeed => 800f;
	protected float MoveMultiplier = 1f;

	private int _playerIndex = 0;
	private Angles _lookAngles;
	private Vector3 _moveInput;

	public Player SelectPlayerIndex( int index )
	{
		var players = GetPlayers().ToList();

		_playerIndex = index;

		if ( _playerIndex >= players.Count )
			_playerIndex = 0;

		if ( _playerIndex < 0 )
			_playerIndex = players.Count - 1;

		var player = players[_playerIndex];
		Target = player;

		// Force freecam off
		IsFree = false;

		return player;
	}

	public Player SpectateNextPlayer( bool asc = true )
	{
		return SelectPlayerIndex( asc ? _playerIndex + 1 : _playerIndex - 1 );
	}

	public void ResetInterpolation()
	{
		// Force eye rotation to avoid lerping when switching targets
		if ( Target.IsValid() )
			Camera.Rotation = Target.EyeRotation;
	}

	protected void ToggleFree()
	{
		IsFree ^= true;

		if ( IsFree )
		{
			if ( Target.IsValid() )
				Camera.Position = Target.EyePosition;

			vm?.Delete();
			cachedCarriable = null;
			Camera.FirstPersonViewer = null;
		}
		else
		{
			ResetInterpolation();
			Camera.FirstPersonViewer = Target;
		}
	}

	float GetSpeedMultiplier()
	{
		if ( Input.Down( InputButton.Run ) )
			return 2f;
		if ( Input.Down( InputButton.Duck ) )
			return 0.3f;

		return 1f;
	}

	public override void BuildInput()
	{
		if ( Input.Pressed( InputButton.Jump ) )
			ToggleFree();

		if ( Input.Pressed( InputButton.Menu ) )
			SpectateNextPlayer( false );

		if ( Input.Pressed( InputButton.Use ) )
			SpectateNextPlayer();

		MoveMultiplier = GetSpeedMultiplier();

		if ( IsFree )
		{
			_moveInput = Input.AnalogMove;
			_lookAngles += Input.AnalogLook;
			_lookAngles.roll = 0;
		}
		else
		{
			base.BuildInput();
		}
	}

	protected BaseViewModel vm;
	protected Carriable cachedCarriable;

	protected void UpdateViewModel( Carriable carriable )
	{
		if ( IsSpectator )
		{
			vm?.Delete();
			vm = null;

			if ( carriable.IsValid() )
			{
				carriable?.CreateViewModel();
				vm = carriable.ViewModelEntity;
			}
		}
		else
		{
			vm?.Delete();
		}
	}

	[GameEvent.Player.SpectatorChanged]
	protected void OnTargetChanged( Player oldTarget, Player newTarget )
	{
		var curWeapon = newTarget?.ActiveCarriable;

		ResetInterpolation();
		UpdateViewModel( curWeapon );
	}

	public override void FrameSimulate()
	{
		if ( !Target.IsValid() )
		{
			IsFree = true;
		}

		if ( IsFree )
		{
			var mv = _moveInput.Normal * BaseMoveSpeed * RealTime.Delta * Camera.Rotation * MoveMultiplier;
			Camera.Position += mv;
			Camera.Rotation = Rotation.From( _lookAngles );
		}
		else
		{
			var curWeapon = Target?.ActiveCarriable;
			if ( curWeapon.IsValid() && curWeapon != cachedCarriable )
			{
				cachedCarriable = curWeapon;
				UpdateViewModel( curWeapon );
			}
		}
	}
}
