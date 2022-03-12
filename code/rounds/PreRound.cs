using System;
using System.Collections.Generic;

using Sandbox;

namespace TTT;

public class PreRound : BaseRound
{
	public override string RoundName => "Preparing";
	public override int RoundDuration => Game.Current.MapSelection.TotalRoundsPlayed == 0 ? Game.PreRoundTime * 2 : Game.PreRoundTime;

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		player.Respawn();
	}

	public override void OnPlayerKilled( Player player )
	{
		StartRespawnTimer( player );

		player.MakeSpectator();

		base.OnPlayerKilled( player );
	}

	protected override void OnStart()
	{
		if ( !Host.IsServer )
			return;

		MapHandler.CleanUp();

		foreach ( Client client in Client.All )
		{
			if ( client.Pawn is Player player )
			{
				player.RemoveLogicButtons();
				player.Respawn();
			}
		}
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		List<Player> players = new();
		List<Player> spectators = new();

		foreach ( Player player in Utils.GetPlayers() )
		{
			player.Client.SetValue( RawStrings.Spectator, player.IsForcedSpectator );
			if ( player.IsForcedSpectator )
			{
				player.MakeSpectator( false );
				spectators.Add( player );
			}
			else
			{
				if ( player.IsAlive() )
					player.Health = player.MaxHealth;
				else
					player.Respawn();

				players.Add( player );
			}
		}

		AssignRolesAndRespawn( players );

		Game.Current.ChangeRound( new InProgressRound
		{
			Players = players,
			Spectators = spectators
		} );
	}

	private static void AssignRolesAndRespawn( List<Player> players )
	{
		int traitorCount = Math.Max( players.Count / 4, 1 );
		int detectiveCount = players.Count / 8;
		players.Shuffle();

		int index = 0;
		while ( traitorCount-- > 0 ) players[index++].SetRole( new TraitorRole() );
		while ( detectiveCount-- > 0 ) players[index++].SetRole( new DetectiveRole() );
		while ( index < players.Count ) players[index++].SetRole( new InnocentRole() );
	}

	private static async void StartRespawnTimer( Player player )
	{
		await GameTask.DelaySeconds( 1 );

		if ( player.IsValid() && Game.Current.Round is PreRound )
			player.Respawn();
	}

	public override void OnPlayerSpawned( Player player )
	{
		player.Inventory.Add( new Hands() );
		base.OnPlayerSpawned( player );
	}
}
