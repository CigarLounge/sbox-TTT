using Sandbox;

namespace TTT;

public class WaitingState : BaseState
{
	public override string Name { get; } = "Waiting";

	public override void OnSecond()
	{
		if ( Game.IsServer && Utils.HasMinimumPlayers() )
			GameManager.Current.ForceStateChange( new PreRound() );
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		player.Respawn();
	}

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		StartRespawnTimer( player );
	}

	protected override void OnStart()
	{
		if ( GameManager.Current.TotalRoundsPlayed != 0 )
			MapHandler.Cleanup();

		if ( !Game.IsServer )
			return;

		foreach ( var client in Game.Clients )
		{
			var player = client.Pawn as Player;
			player.Respawn();
		}
	}
}
