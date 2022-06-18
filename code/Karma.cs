using Sandbox;
using System;

namespace TTT;

public static class Karma
{
	// Maybe turn these values into ServerVars down the line.
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

	private static readonly ColorGroup[] KarmaGroupList = new ColorGroup[]
	{
		new ColorGroup("Liability", Color.FromBytes(255, 130, 0)),
		new ColorGroup("Dangerous", Color.FromBytes(255, 180, 0)),
		new ColorGroup("Trigger-happy", Color.FromBytes(245, 220, 60)),
		new ColorGroup("Crude", Color.FromBytes(255, 240, 135)),
		new ColorGroup("Reputable", Color.FromBytes(255, 255, 255))
	};

	public static ColorGroup GetKarmaGroup( Player player )
	{
		if ( player.BaseKarma >= DefaultValue )
			return KarmaGroupList[^1];

		var index = (int)((player.BaseKarma - MinValue - 1) / ((DefaultValue - MinValue) / KarmaGroupList.Length));
		return KarmaGroupList[index];
	}

	[TTTEvent.Player.Spawned]
	private static void Apply( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( Game.Current.State is not PreRound )
			return;

		if ( !Game.KarmaEnabled || player.BaseKarma >= DefaultValue )
		{
			player.DamageFactor = 1f;
			return;
		}

		var k = player.BaseKarma - DefaultValue;
		var damageFactor = 1 + (0.0007f * k) + (-0.000002f * (k * k));

		player.DamageFactor = Math.Clamp( damageFactor, 0.1f, 1f );
	}

	private static float DecayMultiplier( Player player )
	{
		if ( FallOff <= 0 || player.ActiveKarma < DefaultValue )
			return 1;

		if ( player.ActiveKarma >= MaxValue )
			return 1;

		var baseDiff = MaxValue - DefaultValue;
		var plyDiff = player.ActiveKarma - DefaultValue;
		var half = Math.Clamp( FallOff, 0.1f, 0.99f );

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
		player.ActiveKarma = Math.Max( player.ActiveKarma - penalty, 0 );
		player.TimeUntilClean = Math.Min( Math.Max( player.TimeUntilClean * penalty * 0.2f, penalty ), float.MaxValue );
	}

	private static void GiveReward( Player player, float reward )
	{
		reward = DecayMultiplier( player ) * reward;
		player.ActiveKarma = Math.Min( player.ActiveKarma + reward, MaxValue );
	}

	[TTTEvent.Player.TookDamage]
	private static void OnPlayerTookDamage( Player player )
	{
		if ( Game.Current.State is not InProgress )
			return;

		var attacker = player.LastDamageInfo.Attacker as Player;

		if ( !attacker.IsValid() || !player.IsValid() )
			return;

		if ( attacker == player )
			return;

		var damage = player.LastDamageInfo.Damage;

		if ( attacker.Team == player.Team && player.TimeUntilClean )
		{
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			var penalty = GetHurtPenalty( player.ActiveKarma, damage );
			GivePenalty( attacker, penalty );
		}
		else if ( attacker.Team != Team.Traitors && player.Team == Team.Traitors )
		{
			var reward = GetHurtReward( damage );
			GiveReward( attacker, reward );
		}
	}

	[TTTEvent.Player.Killed]
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

		if ( attacker.Team == player.Team && player.TimeUntilClean )
		{
			/*
			 * If ( WasAvoidable( attacker, victim ) )
			 *		return;
			 */

			var penalty = GetKillPenalty( player.ActiveKarma );
			GivePenalty( attacker, penalty );
		}
		else if ( attacker.Team != Team.Traitors && player.Team == Team.Traitors )
		{
			var reward = GetKillReward();
			GiveReward( attacker, reward );
		}
	}

	private static void RoundIncrement( Player player )
	{
		if ( (!player.IsAlive() && player.DiedBySuicide) || player.IsSpectator )
			return;

		var reward = RoundHeal;

		if ( player.TimeUntilClean )
			reward += CleanBonus;

		GiveReward( player, reward );
	}

	private static bool CheckAutoKick( Player player )
	{
		return Game.KarmaLowAutoKick && player.BaseKarma < MinValue;
	}

	[TTTEvent.Round.Ended]
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
