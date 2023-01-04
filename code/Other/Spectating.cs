using Sandbox;

namespace TTT;

public static class Spectating
{
	/// <summary>
	/// If this is true, we will only be spectating next round.
	/// </summary>	
	public static bool IsForced
	{
		get => forced_spectator;
		internal set
		{
			forced_spectator = value;

			if ( !value || (Game.LocalPawn is Player player && !player.IsAlive) )
				return;

			GameManager.Kill();
		}
	}

	/// <summary>
	/// The player we're currently spectating.
	/// </summary>
	public static Player Player { get; set; }

	// we have to have a backing property since Change doesn't work on clientdata for some reason.
	[ConVar.ClientData] private static bool forced_spectator { get; set; }
	private static int _spectatedPlayerIndex;

	/// <summary>
	/// Swaps the camera target to another alive player.
	/// <para><see cref="bool"/> determines if we go forwards or backwards.</para>
	/// </summary>
	public static void FindPlayer( bool nextPlayer )
	{
		var alivePlayers = Utils.GetPlayersWhere( p => p.IsAlive );
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
