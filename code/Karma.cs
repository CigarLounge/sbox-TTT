using Sandbox;
using System;

namespace TTT;

public static class Karma
{
	// Maybe turn the values into ServerVars down the line.
	public const float CleanBonus = 30;
	public const float DefaultValue = 1000;
	public const float FallOff = 0.25f;
	public const float KillPenalty = 15;
	public const float Ratio = 0.001f;
	public const float RoundHeal = 5;
	public const float TRatio = 0.0003f;
	public const float TBonus = 40;
	public const float MaxValue = 1250;
	public const float MinValue = 450;

	private static readonly string[] KarmaGroupList = new string[]
	{
		"Liability",
		"Dangerous",
		"Trigger-happy",
		"Crude",
		"Reputable",
	};

	public static string GetKarmaGroup( Player player )
	{
		if ( player.CurrentKarma >= DefaultValue )
			return KarmaGroupList[^1];

		var index = (int)(player.CurrentKarma / (DefaultValue / KarmaGroupList.Length - 1));
		return KarmaGroupList[index];
	}

	public static void Apply( Player player )
	{
		if ( !Game.KarmaEnabled || player.BaseKarma >= DefaultValue )
		{
			player.DamageFactor = 1f;
			return;
		}

		float k = player.BaseKarma - DefaultValue;
		float damageFactor;

		damageFactor = 1 + (0.0007f * k) + (-0.000002f * (k * k));

		player.DamageFactor = Math.Clamp( damageFactor, 0.1f, 1f );
	}

	private static float DecayMultiplier( Player player )
	{
		if ( FallOff <= 0 || player.CurrentKarma < DefaultValue )
			return 1;

		if ( player.CurrentKarma >= MaxValue )
			return 1;

		float baseDiff = MaxValue - DefaultValue;
		float plyDiff = player.CurrentKarma - DefaultValue;
		float half = Math.Clamp( FallOff, 0.1f, 0.99f );

		return MathF.Exp( -0.69314718f / (baseDiff * half) * plyDiff );
	}

	private static float GetHurtPenalty( float victimKarma, float damage )
	{
		return victimKarma * Math.Clamp( damage * Ratio, 0, 1 );
	}

	private static float GetKillPenalty( float victimKarma )
	{
		return GetHurtPenalty( victimKarma, KillPenalty );
	}

	/// <summary>
	/// Compute the reward for hurting a traitor.
	/// </summary>
	private static float GetHurtReward( float damage )
	{
		return MaxValue * Math.Clamp( damage * TRatio, 0, 1 );
	}

	/// <summary>
	/// Compute the reward for killing a traitor.
	/// </summary>
	private static float GetKillReward()
	{
		return GetHurtReward( TBonus );
	}

	private static void GivePenalty( Player player, float penalty )
	{
		player.CurrentKarma = Math.Max( player.CurrentKarma - penalty, 0 );
	}

	private static void GiveReward( Player player, float reward )
	{
		reward = DecayMultiplier( player ) * reward;
		player.CurrentKarma = Math.Min( player.CurrentKarma + reward, MaxValue );
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

			float penalty = GetHurtPenalty( player.CurrentKarma, damage );
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

			float penalty = GetKillPenalty( player.CurrentKarma );
			GivePenalty( attacker, penalty );
			attacker.CleanRound = false;
		}
		else if ( attacker.Team != Team.Traitors && player.Team == Team.Traitors )
		{
			float reward = GetKillReward();
			GiveReward( attacker, reward );
		}
	}

	private static void RoundIncrement( Player player )
	{
		// TODO: Figure out a way to check if the player died by suicide.
		if ( player.IsSpectator )
			return;

		float reward = RoundHeal;

		if ( player.CleanRound )
			reward += CleanBonus;

		GiveReward( player, reward );
	}

	private static bool CheckAutoKick( Player player )
	{
		return Game.KarmaLowAutoKick && player.BaseKarma < MinValue;
	}

	public static void OnRoundEnd()
	{
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
		player.BaseKarma = player.CurrentKarma;
	}
}
