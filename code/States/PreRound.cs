using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public class PreRound : BaseState
{
	public override string Name { get; } = "Preparing";
	public override int Duration => Game.Current.TotalRoundsPlayed == 0 ? Game.PreRoundTime * 2 : Game.PreRoundTime;

	public override void OnPlayerSpawned( Player player )
	{
		base.OnPlayerSpawned( player );

		player.Inventory.Add( new Hands() );
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
		MapHandler.Cleanup();

		if ( !Host.IsServer )
			return;

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;
			player.Respawn();
		}
	}

	protected override void OnTimeUp()
	{
		List<Player> players = new();
		List<Player> spectators = new();

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( player.IsForcedSpectator )
			{
				player.Status = PlayerStatus.Spectator;
				player.MakeSpectator( false );
				spectators.Add( player );

				continue;
			}

			if ( player.IsAlive() )
				player.Health = Player.MaxHealth;
			else
				player.Respawn();

			players.Add( player );
		}

		AssignRoles( players );

		Game.Current.ChangeState( new InProgress
		{
			AlivePlayers = players,
			Spectators = spectators,
		} );
	}

	private void AssignRoles( List<Player> players )
	{
		var traitorCount = Math.Max( players.Count >> 2, 1 );
		var detectiveCount = players.Count >> 3;
		players.Shuffle();

		var index = 0;
		var detectiveInfo = GameResource.GetInfo<RoleInfo>( typeof( Detective ) );

		while ( traitorCount-- > 0 )
			players[index++].Role = new Traitor();

		while ( index < players.Count )
		{
			if ( detectiveCount > 0 && players[index].BaseKarma >= detectiveInfo.RequiredKarma )
			{
				players[index].Role = new Detective();
				detectiveCount--;
			}
			else
			{
				players[index].Role = new Innocent();
			}

			index++;
		}
	}
}
