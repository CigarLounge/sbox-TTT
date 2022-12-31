using Sandbox;

namespace TTT;

public abstract partial class CameraMode
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
	private static int _spectatedPlayerIndex;

	/// <summary>
	/// Swaps the camera target to another alive player.
	/// <para><see cref="bool"/> determines if we go forwards or backwards.</para>
	/// </summary>
	public static void SwapSpectatedPlayer( bool nextPlayer )
	{
		var alivePlayers = Utils.GetPlayersWhere( p => p.IsAlive() );
		if ( alivePlayers.IsNullOrEmpty() )
			return;

		_spectatedPlayerIndex += nextPlayer ? 1 : -1;

		if ( _spectatedPlayerIndex >= alivePlayers.Count )
			_spectatedPlayerIndex = 0;
		else if ( _spectatedPlayerIndex < 0 )
			_spectatedPlayerIndex = alivePlayers.Count - 1;

		Target = alivePlayers[_spectatedPlayerIndex];
	}

	/// <summary>
	/// Any camera inputs that need to happen every frame.
	/// </summary>
	public virtual void BuildInput( Player player ) { }

	/// <summary>
	/// Update the camera position here since it happens every frame.
	/// </summary>
	public virtual void FrameSimulate( Player player ) { }
}
