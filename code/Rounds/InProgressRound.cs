using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class InProgressRound : BaseRound
{
	[Net]
	public List<Player> Players { get; set; }

	[Net]
	public List<Player> Spectators { get; set; }

	[Net]
	public TimeUntil TimeUntilExpectedRoundEnd { get; set; }

	public string TimeUntilExpectedRoundEndFormatted => (int)TimeUntilExpectedRoundEnd < 0 ?
														$"+{TimeUntilExpectedRoundEnd.Relative.TimerString()}"
														: TimeUntilExpectedRoundEnd.Relative.TimerString();

	public override string RoundName => "In Progress";
	public override int RoundDuration => Game.InProgressRoundTime;

	private readonly List<RoleButton> _logicButtons = new();

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		Players.Remove( player );
		Spectators.AddIfDoesNotContain( player );

		Karma.OnPlayerKilled( player );
		player.UpdateMissingInAction();
		ChangeRoundIfOver();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		Spectators.AddIfDoesNotContain( player );
		SyncPlayer( player );
	}

	public override void OnPlayerLeave( Player player )
	{
		base.OnPlayerLeave( player );

		Players.Remove( player );
		Spectators.Remove( player );

		ChangeRoundIfOver();
	}

	protected override void OnStart()
	{
		if ( Host.IsClient && Local.Pawn is Player localPlayer )
		{
			UI.InfoFeed.Instance?.AddEntry( $"The round has begun! Haste mode is enabled giving Traitors {Game.InProgressSecondsPerDeath} seconds per death!" );

			var karma = (int)localPlayer.Client.GetValue<float>( Strings.Karma );
			UI.InfoFeed.Instance?.AddEntry( karma >= 1000 ?
											$"Your karma is {karma}, so you'll deal full damage this round!" :
											$"Your karma is {karma}, so you'll deal reduced damage this round!" );
		}

		if ( !Host.IsServer )
			return;

		TimeUntilExpectedRoundEnd = TimeUntilRoundEnd;

		// For now, if the RandomWeaponCount of the map is zero, let's just give the players
		// a fixed weapon loadout.
		if ( MapHandler.RandomWeaponCount == 0 )
		{
			foreach ( Player player in Players )
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

		LoadPostRound( Team.Innocents );
	}

	private Team IsRoundOver()
	{
		List<Team> aliveTeams = new();

		foreach ( var player in Players )
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
		Game.Current.ForceRoundChange( new PostRound() );

		UI.PostRoundMenu.DisplayWinner( winningTeam );
	}

	public override void OnSecond()
	{
		if ( !Host.IsServer )
			return;

		if ( !Game.PreventWin )
			base.OnSecond();
		else
			TimeUntilRoundEnd += 1f;

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

	[TTTEvent.Player.RoleChanged]
	private static void OnPlayerRoleChange( Player player, BaseRole oldRole )
	{
		if ( Host.IsClient )
			return;

		if ( Game.Current.Round is InProgressRound inProgressRound )
			inProgressRound.ChangeRoundIfOver();
	}
}
