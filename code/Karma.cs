using Sandbox;
using System;

namespace TTT;

public static class Karma
{
	// Maybe turn the values into ServerVars down the line.
	public const float CleanBonus = 30;
	public const float DefaultValue = 1000;
	public const float FallOff = 0.25f;
	public const float KillPenalty = 15f;
	public const float Ratio = 0.001f;
	public const float RoundHeal = 5f;
	public const float TRatio = 0.0003f;
	public const float TBonus = 40f;
	public const float MaxValue = 1250;
	public const float MinValue = 450;

	public static bool IsEnabled => Game.KarmaEnabled;

	public static void Apply( Player player )
	{
		if ( !IsEnabled || player.BaseKarma >= 1000 )
		{
			player.DamageFactor = 1f;
			return;
		}

		float damageFactor = 1f;
		float k = player.BaseKarma - 1000;

		if ( Game.KarmaStrict )
			damageFactor *= 1 + (0.0007f * k) + (-0.000002f * (k * k));
		else
			damageFactor = 1 + -0.0000025f * (k * k);

		damageFactor = Math.Clamp( damageFactor, 0.1f, 1f );

		player.DamageFactor = damageFactor;
	}

	public static float DecayMultiplier( Player player )
	{
		if ( FallOff <= 0 || player.LiveKarma < DefaultValue )
			return 1;

		if ( player.LiveKarma < MaxValue )
		{
			float baseDiff = MaxValue - DefaultValue;
			float plyDiff = player.LiveKarma - DefaultValue;
			float half = Math.Clamp( FallOff, 0.1f, 0.99f );

			return MathF.Exp( -0.69314718f / baseDiff * half * plyDiff );
		}

		return 1;
	}

	public static float GetHurtPenalty( float victimKarma, float damage )
	{
		return victimKarma * Math.Clamp( damage * Ratio, 0, 1 );
	}

	public static float GetKillPenalty( float victimKarma )
	{
		return GetHurtPenalty( victimKarma, KillPenalty );
	}

	/// <summary>
	/// Compute the reward for hurting a traitor.
	/// </summary>
	public static float GetHurtReward( float damage )
	{
		return MaxValue * Math.Clamp( damage * TRatio, 0, 1 );
	}

	/// <summary>
	/// Compute the reward for killing a traitor.
	/// </summary>
	public static float GetKillReward()
	{
		return GetHurtReward( TBonus );
	}

	public static void GivePenalty( Player player, float penalty )
	{
		player.LiveKarma = Math.Max( player.LiveKarma - penalty, 0 );
	}

	public static void GiveReward( Player player, float reward )
	{
		reward = DecayMultiplier( player ) * reward;
		player.LiveKarma = Math.Min( player.LiveKarma + reward, MaxValue );
	}


	public static void OnPlayerHurt( Player player )
	{
		var attacker = player.LastDamageInfo.Attacker as Player;

		if ( !attacker.IsValid() || !player.IsValid() )
			return;

		if ( attacker == player )
			return;

		float damage = Math.Min( player.Health, player.LastDamageInfo.Damage );

		if ( attacker.Team == player.Team )
		{
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			float penalty = GetHurtPenalty( player.LiveKarma, damage );
			GivePenalty( attacker, penalty );
			attacker.CleanRound = false;
		}
		else if ( attacker.Team != Team.Traitors && player.Team == Team.Traitors )
		{
			float reward = GetHurtReward( damage );
			GiveReward( attacker, reward );
		}
	}

	public static void OnPlayerKilled( Player player )
	{
		var attacker = player.LastAttacker as Player;

		if ( !attacker.IsValid() || !player.IsValid() )
			return;

		if ( attacker == player )
			return;

		if ( attacker.Team == player.Team )
		{
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			float penalty = GetKillPenalty( player.LiveKarma );
			GivePenalty( attacker, penalty );
			attacker.CleanRound = false;
		}
		else if ( attacker.Team != Team.Traitors && player.Team == Team.Traitors )
		{
			float reward = GetKillReward();
			GiveReward( attacker, reward );
		}
	}

	public static void RoundIncrement( Player player )
	{
		float reward = RoundHeal;

		if ( player.CleanRound )
			reward += CleanBonus;

		GiveReward( player, reward );
	}

	public static bool CheckAutoKick( Player player )
	{
		return player.BaseKarma < MinValue;
	}

	public static void OnRoundBegin()
	{
		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;
			Apply( player );
		}
	}

	public static void OnRoundEnd()
	{
		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			RoundIncrement( player );
			Rebase( player );

			if ( IsEnabled && CheckAutoKick( player ) )
				client.Kick();
		}
	}

	public static void Rebase( Player player )
	{
		player.BaseKarma = player.LiveKarma;
	}
}
