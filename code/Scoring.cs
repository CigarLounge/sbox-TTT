using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public static class Scoring
{
	public struct Config
	{
		public Config() { }

		[Description( "The amount of score points rewarded for confirming a corpse." )]
		[Property]
		public int CorpseFoundReward { get; set; } = 1;

		[Description( "The amount of score points rewarded for killing a player with this role." )]
		[Property]
		public int KillReward { get; set; } = 1;

		[Description( "The amount of score points penalized for killing a player on the same team." )]
		[Property]
		public int TeamKillPenalty { get; set; } = 8;

		[Description( "The amount of score points rewarded for surviving the round." )]
		[Property]
		public int SurviveBonus { get; set; } = 1;

		[Description( "The amount of score points penalized for commiting suicide." )]
		[Property]
		public int SuicidePenalty { get; set; } = 1;
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( Game.Current.State is not InProgress )
			return;

		if ( player.DiedBySuicide )
		{
			player.RoundScore -= player.Role.Scoring.SuicidePenalty;
		}
		else
		{
			var attacker = (Player)player.LastAttacker;

			if ( attacker.Team != player.Team )
				attacker.RoundScore += player.Role.Scoring.KillReward;
			else
				attacker.RoundScore -= attacker.Role.Scoring.TeamKillPenalty;
		}
	}

	[TTTEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player player )
	{
		if ( !Host.IsServer )
			return;

		var confirmer = player.Confirmer;
		confirmer.RoundScore += confirmer.Role.Scoring.CorpseFoundReward;
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

			player.RoundScore += player.Role.Scoring.SurviveBonus;
			alivePlayersCount[(int)player.Team]++;
		}

		var traitorBonus = (int)MathF.Ceiling( deadPlayersCount[1] / 2f );
		var innocentBonus = alivePlayersCount[1];

		if ( winType != WinType.TimeUp )
			traitorBonus += alivePlayersCount[2];
		else
			traitorBonus -= (int)MathF.Floor( alivePlayersCount[1] / 2f );

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			var bonus = player.Team == Team.Traitors ? traitorBonus : innocentBonus;
			player.RoundScore += bonus;

			// Add the score gained this round to the actual score.
			player.Score += player.RoundScore;
			player.RoundScore = 0;
		}
	}
}
