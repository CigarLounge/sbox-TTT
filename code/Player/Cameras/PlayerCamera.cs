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

			_spectatedPlayer = value == Game.LocalPawn ? null : value;
		}
	}
	private static Player _spectatedPlayer;

	private bool _isThirdPerson = false;
	private int _spectatedPlayerIndex = 0;

	public PlayerCamera() { }

	public PlayerCamera( Player targetPlayer ) => Target = targetPlayer;

	public override void BuildInput( Player player )
	{
		if ( player.IsAlive() )
			return;

		if ( Input.Pressed( InputButton.Jump ) )
		{
			if ( !_isThirdPerson )
				_isThirdPerson = true;
			else
				player.CurrentCamera = new FreeCamera();
		}

		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			SwapSpectatedPlayer( false );

		if ( Input.Pressed( InputButton.SecondaryAttack ) )
			SwapSpectatedPlayer( true );
	}

	public override void FrameSimulate( Player player )
	{
		if ( _isThirdPerson )
		{
			var tr = Trace.Ray( Target.Position, Target.Position + player.ViewAngles.ToRotation().Forward * -130 + Vector3.Up * player.EyeLocalPosition )
				.WorldOnly()
				.Run();

			Camera.Rotation = player.ViewAngles.ToRotation();
			Camera.Position = tr.EndPosition;
			Camera.FirstPersonViewer = null;
		}
		else
		{
			Camera.Position = Target.EyePosition;
			Camera.Rotation = IsSpectatingPlayer ? Rotation.Slerp( Camera.Rotation, Target.EyeLocalRotation, Time.Delta * 20f ) : Target.EyeRotation;
			Camera.FirstPersonViewer = Target;
		}

		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );
		Camera.Main.SetViewModelCamera( Camera.FieldOfView );
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
