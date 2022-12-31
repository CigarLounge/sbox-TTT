using Sandbox;

namespace TTT;

public static class Spectating
{
	public static Player Player { get; internal set; }

	private static int _spectatedPlayerIndex;

	/// <summary>
	/// Swaps the camera target to another alive player.
	/// <para><see cref="bool"/> determines if we go forwards or backwards.</para>
	/// </summary>
	public static void FindPlayer( bool nextPlayer )
	{
		var alivePlayers = Utils.GetAlivePlayers();
		if ( alivePlayers.IsNullOrEmpty() )
			return;

		_spectatedPlayerIndex += nextPlayer ? 1 : -1;

		if ( _spectatedPlayerIndex >= alivePlayers.Count )
			_spectatedPlayerIndex = 0;
		else if ( _spectatedPlayerIndex < 0 )
			_spectatedPlayerIndex = alivePlayers.Count - 1;

		Player = alivePlayers[_spectatedPlayerIndex];
	}

	[GameEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( player == Player )
			CameraMode.Current = new FreeCamera();
	}
}
