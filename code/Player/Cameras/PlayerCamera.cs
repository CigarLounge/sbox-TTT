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

	public override void FrameSimulate( Player player )
	{
		Camera.Position = Target.EyePosition;
		Camera.Rotation = Target.EyeRotation;
		Camera.FirstPersonViewer = Target;
	}
}
