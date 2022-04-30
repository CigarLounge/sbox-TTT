using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public class PreRound : BaseState
{
	public override string Name => "Preparing";
	public override int Duration => Game.Current.TotalRoundsPlayed == 0 ? Game.PreRoundTime * 2 : Game.PreRoundTime;

	public override void OnPlayerSpawned( Player player )
	{
		base.OnPlayerSpawned( player );

		Karma.Apply( player );
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
		base.OnStart();

		MapHandler.CleanUp();
		Event.Run( TTTEvent.Round.Started );

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
		base.OnTimeUp();

		List<Player> players = new();
		List<Player> spectators = new();

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;
			player.IsSpectator = player.IsForcedSpectator;

			if ( player.IsForcedSpectator )
			{
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

		(var innocents, var detectives, var traitors) = AssignRoles( players );

		Game.Current.ChangeRound( new InProgress
		{
			AlivePlayers = players,
			Spectators = spectators,
			InnocentClientIds = innocents.ToArray(),
			DetectiveClientIds = detectives.ToArray(),
			TraitorClientIds = traitors.ToArray()
		} );
	}

	private (List<long>, List<long>, List<long>) AssignRoles( List<Player> players )
	{
		List<long> innocents = new();
		List<long> detectives = new();
		List<long> traitors = new();

		int traitorCount = Math.Max( players.Count >> 2, 1 );
		int detectiveCount = players.Count >> 3;
		players.Shuffle();

		int index = 0;
		while ( traitorCount-- > 0 )
		{
			var player = players[index++];
			player.Role = new Traitor();
			traitors.Add( player.Client.PlayerId );
		}

		while ( detectiveCount-- > 0 )
		{
			var player = players[index++];
			player.Role = new Detective();
			detectives.Add( player.Client.PlayerId );
		}

		while ( index < players.Count )
		{
			var player = players[index++];
			player.Role = new Innocent();
			innocents.Add( player.Client.PlayerId );
		}

		return (innocents, detectives, traitors);
	}
}
