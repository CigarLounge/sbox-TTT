using Sandbox;

namespace TTT;

public class WaitingState : BaseState
{
	public override string Name { get; } = "Waiting";

	public override void OnSecond()
	{
		if ( Host.IsServer && Utils.HasMinimumPlayers() )
			Game.Current.ForceStateChange( new PreRound() );
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
		if ( !Host.IsServer )
			return;

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;
			player.Respawn();
		}
	}
}
