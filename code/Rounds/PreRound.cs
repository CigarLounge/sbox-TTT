using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public class PreRound : BaseRound
{
	public override string RoundName => "Preparing";
	public override int RoundDuration => Game.Current.TotalRoundsPlayed == 0 ? Game.PreRoundTime * 2 : Game.PreRoundTime;

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

		if ( !Host.IsServer )
			return;

		Karma.OnRoundBegin();

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
			player.Client.SetValue( RawStrings.Spectator, player.IsForcedSpectator );

			if ( player.IsForcedSpectator )
			{
				player.MakeSpectator( false );
				spectators.Add( player );

				continue;
			}

			if ( player.IsAlive() )
				player.Health = player.MaxHealth;
			else
				player.Respawn();

			players.Add( player );
		}

		AssignRoles( players );

		Game.Current.ChangeRound( new InProgressRound
		{
			Players = players,
			Spectators = spectators
		} );
	}

	private void AssignRoles( List<Player> players )
	{
		int traitorCount = Math.Max( players.Count >> 2, 1 );
		int detectiveCount = players.Count >> 3;
		players.Shuffle();

		int index = 0;
		while ( traitorCount-- > 0 ) players[index++].SetRole( new Traitor() );
		while ( detectiveCount-- > 0 ) players[index++].SetRole( new Detective() );
		while ( index < players.Count ) players[index++].SetRole( new Innocent() );
	}

	private static async void StartRespawnTimer( Player player )
	{
		await GameTask.DelaySeconds( 1 );

		if ( player.IsValid() && Game.Current.Round is PreRound )
			player.Respawn();
	}

	public override void OnPlayerSpawned( Player player )
	{
		base.OnPlayerSpawned( player );

		player.Inventory.Add( new Hands() );
	}
}
