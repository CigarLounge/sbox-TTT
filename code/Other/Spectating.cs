using Sandbox;

namespace TTT;

public static class Spectating
{
	[ConVar.ClientData] private static bool forced_spectator { get; set; }
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

	private static int _spectatedPlayerIndex;

	/// <summary>
	/// Cycles through player list to find a spectating target.
	/// </summary>
	/// <param name="forward">Determines if we cycle forwards or backwards.</param>
	public static void FindPlayer( bool forward )
	{
		var alivePlayers = Utils.GetPlayersWhere( p => p.IsAlive );

		if ( alivePlayers.IsNullOrEmpty() )
			return;

		_spectatedPlayerIndex += forward ? 1 : -1;

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
