using Sandbox;
using System;

namespace TTT;

public static class Karma
{
	public const float CleanBonus = 30;
	public const float FallOff = 0.25f;
	public const float RoundHeal = 5;

	private static readonly ColorGroup[] _karmaGroupList = new ColorGroup[]
	{
		new ColorGroup("Liability", Color.FromBytes(255, 130, 0)),
		new ColorGroup("Dangerous", Color.FromBytes(255, 180, 0)),
		new ColorGroup("Trigger-happy", Color.FromBytes(245, 220, 60)),
		new ColorGroup("Crude", Color.FromBytes(255, 240, 135)),
		new ColorGroup("Reputable", Color.FromBytes(255, 255, 255))
	};

	public static float GetHurtReward( float damage, float multiplier )
	{
		return Game.KarmaMaxValue * Math.Clamp( damage * multiplier, 0, 1 );
	}

	public static float GetHurtPenalty( float victimKarma, float damage, float multiplier )
	{
		return victimKarma * Math.Clamp( damage * multiplier, 0, 1 );
	}

	public static float GetKillReward( float multiplier )
	{
		return Game.KarmaMaxValue * Math.Clamp( multiplier, 0, 1 );
	}

	public static float GetKillPenalty( float victimKarma, float multiplier )
	{
		return victimKarma * Math.Clamp( multiplier, 0, 1 );
	}

	private static void GivePenalty( Player player, float penalty )
	{
		player.ActiveKarma = Math.Max( player.ActiveKarma - penalty, 0 );
		player.TimeUntilClean = Math.Min( Math.Max( player.TimeUntilClean * penalty * 0.2f, penalty ), float.MaxValue );
	}

	private static void GiveReward( Player player, float reward )
	{
		reward = DecayMultiplier( player ) * reward;
		player.ActiveKarma = Math.Min( player.ActiveKarma + reward, Game.KarmaMaxValue );
	}

	private static float DecayMultiplier( Player player )
	{
		if ( FallOff <= 0 || player.ActiveKarma < Game.KarmaStartValue )
			return 1;

		if ( player.ActiveKarma >= Game.KarmaMaxValue )
			return 1;

		var baseDiff = Game.KarmaMaxValue - Game.KarmaStartValue;
		var plyDiff = player.ActiveKarma - Game.KarmaStartValue;
		var half = Math.Clamp( FallOff, 0.1f, 0.99f );

		return MathF.Exp( -0.69314718f / (baseDiff * half) * plyDiff );
	}

	public static ColorGroup GetKarmaGroup( Player player )
	{
		if ( player.BaseKarma >= Game.KarmaStartValue )
			return _karmaGroupList[^1];

		var index = (int)((player.BaseKarma - Game.KarmaMinValue - 1) / ((Game.KarmaStartValue - Game.KarmaMinValue) / _karmaGroupList.Length));
		return _karmaGroupList[index];
	}

	[GameEvent.Player.Spawned]
	private static void Apply( Player player )
	{
		if ( Game.Current.State is not PreRound )
			return;

		player.TimeUntilClean = 0;

		if ( !Game.KarmaEnabled || player.BaseKarma >= Game.KarmaStartValue )
		{
			player.DamageFactor = 1f;
			return;
		}

		var k = player.BaseKarma - Game.KarmaStartValue;
		var damageFactor = 1 + (0.0007f * k) + (-0.000002f * (k * k));

		player.DamageFactor = Math.Clamp( damageFactor, 0.1f, 1f );
	}


	[GameEvent.Player.TookDamage]
	private static void OnPlayerTookDamage( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( Game.Current.State is not InProgress )
			return;

		var attacker = player.LastAttacker as Player;

		if ( !attacker.IsValid() || !player.IsValid() )
			return;

		if ( attacker == player )
			return;

		var damage = player.LastDamage.Damage;

		if ( attacker.Team == player.Team )
		{
			if ( !player.TimeUntilClean )
				return;
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			var penalty = GetHurtPenalty( player.ActiveKarma, damage, attacker.Role.Karma.TeamHurtPenaltyMultiplier );
			GivePenalty( attacker, penalty );
		}
		else
		{
			var reward = GetHurtReward( damage, player.Role.Karma.AttackerHurtRewardMultiplier );
			GiveReward( attacker, reward );
		}
	}

	[GameEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( Game.Current.State is not InProgress )
			return;

		var attacker = player.LastAttacker as Player;

		if ( !attacker.IsValid() || !player.IsValid() )
			return;

		if ( attacker == player )
			return;

		if ( attacker.Team == player.Team )
		{
			if ( !player.TimeUntilClean )
				return;
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			var penalty = GetKillPenalty( player.ActiveKarma, attacker.Role.Karma.TeamKillPenaltyMultiplier );
			GivePenalty( attacker, penalty );
		}
		else
		{
			var reward = GetKillReward( player.Role.Karma.AttackerKillRewardMultiplier );
			GiveReward( attacker, reward );
		}
	}

	private static void RoundIncrement( Player player )
	{
		if ( (!player.IsAlive() && !player.KilledByPlayer) || player.IsSpectator )
			return;

		var reward = RoundHeal;

		if ( player.TimeUntilClean )
			reward += CleanBonus;

		GiveReward( player, reward );
	}

	private static bool CheckAutoKick( Player player )
	{
		return Game.KarmaLowAutoKick && player.BaseKarma < Game.KarmaMinValue;
	}

	[GameEvent.Round.End]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( !Host.IsServer )
			return;

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			RoundIncrement( player );
			Rebase( player );

			if ( Game.KarmaEnabled && CheckAutoKick( player ) )
				client.Kick();
		}
	}

	private static void Rebase( Player player )
	{
		player.BaseKarma = player.ActiveKarma;
	}
}
