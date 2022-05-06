using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public static class Scoring
{
	public static void OnPlayerKilled( Player player )
	{
		if ( player.DiedBySuicide )
		{
			player.RoundScore--;
		}
		else if ( player.LastAttacker is Player attacker )
		{
			if ( attacker.Team != player.Team )
				attacker.RoundScore += attacker.Team == Team.Traitors ? 1 : 5;
			else
				attacker.RoundScore -= attacker.Team == Team.Traitors ? 16 : 8;
		}
	}

	[TTTEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player player )
	{
		if ( !Host.IsServer )
			return;

		var confirmer = player.Confirmer;

		if ( confirmer.Team != Team.Traitors )
			confirmer.RoundScore += confirmer.Role is Detective ? 3 : 1;
	}

	[TTTEvent.Round.Ended]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( !Host.IsServer )
			return;

		var alivePlayersCount = new List<int>( new int[3] );
		var deadPlayersCount = new List<int>( new int[3] );

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( !player.IsAlive() )
			{
				deadPlayersCount[(int)player.Team]++;
				continue;
			}

			player.RoundScore++;
			alivePlayersCount[(int)player.Team]++;
		}

		int traitorBonus = (int)MathF.Ceiling( deadPlayersCount[1] / 2f );
		int innocentBonus = alivePlayersCount[1];

		if ( winType != WinType.TimeUp )
			traitorBonus += alivePlayersCount[2];
		else
			traitorBonus -= (int)MathF.Floor( alivePlayersCount[1] / 2f );

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			int bonus = player.Team == Team.Traitors ? traitorBonus : innocentBonus;
			player.RoundScore += bonus;

			// Add the score gained this round to the actual score.
			player.Score += player.RoundScore;
			player.RoundScore = 0;
		}
	}
}
