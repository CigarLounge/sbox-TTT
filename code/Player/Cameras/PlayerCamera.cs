using Sandbox;

namespace TTT;

public class PlayerCamera : BaseCamera
{
	public static bool IsSpectatingPlayer => _spectatedPlayer.IsValid();

	/// <summary>
	/// The current Target we are viewing, can be either the LocalPawn or another player.
	/// </summary>
	public static Player Target
	{
		get => _spectatedPlayer ?? Game.LocalPawn as Player;
		set
		{
			if ( value == _spectatedPlayer )
				return;

			var oldSpectatedPlayer = _spectatedPlayer;
			_spectatedPlayer = value == Game.LocalPawn ? null : value;
			Event.Run( GameEvent.Player.SpectatorChanged, oldSpectatedPlayer, _spectatedPlayer );
		}
	}
	private static Player _spectatedPlayer;
	private int _spectatedPlayerIndex = 0;

	public PlayerCamera() { }

	public PlayerCamera( Player targetPlayer ) => Target = targetPlayer;

	public override void BuildInput( Player player )
	{
		if ( player.IsAlive() )
			return;

		if ( Input.Pressed( InputButton.Jump ) )
			player.CurrentCamera = new FreeCamera();

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			SwapSpectatedPlayer( false );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
			SwapSpectatedPlayer( true );
	}

	public override void FrameSimulate( Player player )
	{
		Camera.Position = Target.EyePosition;
		Camera.Rotation = IsSpectatingPlayer ? Rotation.Slerp( Camera.Rotation, Target.EyeLocalRotation, Time.Delta * 20f ) : Target.EyeRotation;
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.Main.SetViewModelCamera( Camera.FieldOfView );
		Camera.FirstPersonViewer = Target;
	}

	private void SwapSpectatedPlayer( bool nextPlayer )
	{
		var alivePlayers = Utils.GetAlivePlayers();
		if ( alivePlayers.IsNullOrEmpty() )
			return;

		_spectatedPlayerIndex += nextPlayer ? 1 : -1;

		if ( _spectatedPlayerIndex >= alivePlayers.Count )
			_spectatedPlayerIndex = 0;
		else if ( _spectatedPlayerIndex < 0 )
			_spectatedPlayerIndex = alivePlayers.Count - 1;

		Target = alivePlayers[_spectatedPlayerIndex];
	}
}
