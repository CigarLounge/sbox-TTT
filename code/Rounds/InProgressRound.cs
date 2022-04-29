using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public partial class InProgressRound : BaseRound
{
	public List<Player> AlivePlayers { get; set; }
	public List<Player> Spectators { get; set; }

	public Player[] Innocents { get; set; }
	public Player[] Detectives { get; set; }
	public Player[] Traitors { get; set; }

	/// <summary>
	/// Unique case where InProgressRound has a seperate fake timer for Innocents.
	/// The real timer is only displayed to Traitors as it increments every player death during the round.
	/// </summary>
	[Net]
	public TimeUntil FakeTime { get; private set; }
	public string FakeTimeFormatted => FakeTime.Relative.TimerString();

	public override string RoundName => "In Progress";
	public override int RoundDuration => Game.InProgressRoundTime;

	private int InnocentTeamDeathCount { get; set; }
	private readonly List<RoleButton> _logicButtons = new();
	private bool _timeUp = false;

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		TimeLeft += Game.InProgressSecondsPerDeath;

		ApplyScoring( player );

		if ( player.Team == Team.Innocents )
			InnocentTeamDeathCount += 1;

		float percentDead = (float)InnocentTeamDeathCount / (Innocents.Length + Detectives.Length);
		if ( percentDead >= Game.CreditsAwardPercentage )
		{
			GivePlayersCredits( new Traitor(), Game.CreditsAwarded );
			InnocentTeamDeathCount = 0;
		}

		if ( player.Role is Traitor )
			GivePlayersCredits( new Detective(), Game.DetectiveTraitorDeathReward );
		else if ( player.Role is Detective && player.LastAttacker is Player p && p.IsAlive() && p.Team == Team.Traitors )
			GiveTraitorCredits( p );

		AlivePlayers.Remove( player );
		Spectators.Add( player );

		Karma.OnPlayerKilled( player );
		player.UpdateMissingInAction();
		ChangeRoundIfOver();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		Spectators.Add( player );
		SyncPlayer( player );
	}

	public override void OnPlayerLeave( Player player )
	{
		base.OnPlayerLeave( player );

		AlivePlayers.Remove( player );
		Spectators.Remove( player );

		ChangeRoundIfOver();
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( Host.IsClient && Local.Pawn is Player localPlayer )
		{
			UI.InfoFeed.Instance?.AddEntry( "Roles have been selected and the round has begun..." );
			UI.InfoFeed.Instance?.AddEntry( $"Traitors will receive an additional {Game.InProgressSecondsPerDeath} seconds per death." );

			float karma = MathF.Round( localPlayer.BaseKarma );
			UI.InfoFeed.Instance?.AddEntry( karma >= 1000 ?
											$"Your karma is {karma}, so you'll deal full damage this round." :
											$"Your karma is {karma}, so you'll deal reduced damage this round." );

			return;
		}

		FakeTime = TimeLeft;

		// For now, if the RandomWeaponCount of the map is zero, let's just give the players
		// a fixed weapon loadout.
		if ( MapHandler.RandomWeaponCount == 0 )
		{
			foreach ( var player in AlivePlayers )
			{
				GiveFixedLoadout( player );
			}
		}

		foreach ( var ent in Entity.All )
		{
			if ( ent is RoleButton button )
				_logicButtons.Add( button );
			else if ( ent is Corpse corpse )
				corpse.Delete();
		}
	}

	private void GiveFixedLoadout( Player player )
	{
		if ( player.Inventory.Add( new MP5() ) )
			player.GiveAmmo( AmmoType.PistolSMG, 120 );

		if ( player.Inventory.Add( new Revolver() ) )
			player.GiveAmmo( AmmoType.Magnum, 20 );
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		_timeUp = true;
		LoadPostRound( Team.Innocents );
	}

	private Team IsRoundOver()
	{
		List<Team> aliveTeams = new();

		foreach ( var player in AlivePlayers )
		{
			if ( !aliveTeams.Contains( player.Team ) )
				aliveTeams.Add( player.Team );
		}

		if ( aliveTeams.Count == 0 )
			return Team.None;

		return aliveTeams.Count == 1 ? aliveTeams[0] : Team.None;
	}

	public void LoadPostRound( Team winningTeam )
	{
		Game.Current.TotalRoundsPlayed++;

		HandleTeamBonus();

		Game.Current.ForceRoundChange( new PostRound() );

		UI.PostRoundPopup.DisplayWinner( winningTeam );
		UI.GeneralMenu.LoadPlayerData( Innocents, Detectives, Traitors );
	}

	public override void OnSecond()
	{
		if ( !Host.IsServer )
			return;

		if ( Game.PreventWin )
			TimeLeft += 1f;

		if ( TimeLeft )
			OnTimeUp();

		_logicButtons.ForEach( x => x.OnSecond() ); // Tick role button delay timer.

		if ( !Utils.HasMinimumPlayers() && IsRoundOver() == Team.None )
			Game.Current.ForceRoundChange( new WaitingRound() );
	}

	private bool ChangeRoundIfOver()
	{
		var result = IsRoundOver();

		if ( result != Team.None && !Game.PreventWin )
		{
			LoadPostRound( result );
			return true;
		}

		return false;
	}

	private void GivePlayersCredits( BaseRole role, int credits )
	{
		var clients = Utils.GetAliveClientsWithRole( role );

		clients.ForEach( ( cl ) =>
		{
			if ( cl.Pawn is Player p )
				p.Credits += credits;
		} );
		UI.InfoFeed.DisplayRoleEntry
		(
			To.Multiple( clients ),
			Asset.GetInfo<RoleInfo>( role.Title ),
			$"You have been awarded {credits} credits for your performance."
		);
	}

	private void GiveTraitorCredits( Player traitor )
	{
		traitor.Credits += Game.TraitorDetectiveKillReward;
		UI.InfoFeed.DisplayClientEntry( To.Single( traitor.Client ), $"have received {Game.TraitorDetectiveKillReward} credits for killing a Detective" );
	}

	private void HandleTeamBonus()
	{
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

		if ( !_timeUp )
			traitorBonus += alivePlayersCount[2];
		else
			traitorBonus -= (int)MathF.Floor( alivePlayersCount[1] / 2f );

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( player.Team == Team.Innocents )
				player.RoundScore += innocentBonus;
			else if ( player.Team == Team.Traitors )
				player.RoundScore += traitorBonus;
		}
	}

	private void ApplyScoring( Player player )
	{
		if ( player.DiedBySuicide )
		{
			player.RoundScore -= 1;
		}
		else if ( player.LastAttacker is Player attacker )
		{
			if ( attacker.Team != player.Team )
				attacker.RoundScore += attacker.Team == Team.Traitors ? 1 : 5;
			else
				attacker.RoundScore -= attacker.Team == Team.Traitors ? 16 : 8;
		}
	}

	[TTTEvent.Player.RoleChanged]
	private static void OnPlayerRoleChange( Player player, BaseRole oldRole )
	{
		if ( Host.IsClient )
			return;

		if ( Game.Current.Round is InProgressRound inProgressRound )
			inProgressRound.ChangeRoundIfOver();
	}
}
