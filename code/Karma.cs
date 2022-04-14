using Sandbox;
using System;

namespace TTT;

public static class Karma
{
	// Maybe turn the values into ServerVars down the line.
	public const int DefaultValue = 1000;
	public const float KillPenalty = 15f;
	public const float Ratio = 0.001f;
	public const float TRatio = 0.0003f;
	public const float TBonus = 40f;
	public const int MaxValue = 1000;
	public const int MinValue = 450;

	public static bool IsEnabled => Game.KarmaEnabled;

	public static float ApplyKarma( Player player, float damage )
	{
		if ( !IsEnabled )
			return damage;

		if ( player.Client.GetInt( "karma" ) >= 1000 )
			return damage;

		float damageFactor = 1f;
		float k = player.Client.GetInt( "karma" ) - 1000;

		if ( Game.KarmaStrict )
			damageFactor *= 1 + (0.0007f * k) + (-0.000002f * (k * k));
		else
			damageFactor = 1 + -0.0000025f * (k * k);

		damageFactor = Math.Clamp( damageFactor, 0.1f, 1f );

		return damageFactor;
	}

	public static float GetHurtPenalty( float victimKarma, float damage )
	{
		return victimKarma * Math.Clamp( damage * Ratio, 0, 1 );
	}

	public static float GetKillPenalty( float victimKarma )
	{
		return GetHurtPenalty( victimKarma, KillPenalty );
	}

	public static float GetHurtReward( float damage )
	{
		return MaxValue * Math.Clamp( damage * TRatio, 0, 1 );
	}

	public static float GetKillReward()
	{
		return GetHurtReward( TBonus );
	}

	public static void OnPlayerKilled( Player attacker, Player victim )
	{
		if ( !attacker.IsValid() || !victim.IsValid() )
			return;

		if ( attacker == victim )
			return;

		if ( attacker.Team == victim.Team )
		{

		}
		else
		{
			float reward = GetKillReward();
		}
	}

	public static bool CheckAutoKick( Client client )
	{
		return client.GetInt( "karma" ) < MinValue;
	}
}
