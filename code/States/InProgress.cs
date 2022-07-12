using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class InProgress : BaseState
{
	public List<Player> AlivePlayers { get; set; }
	public List<Player> Spectators { get; set; }

	/// <summary>
	/// Unique case where InProgress has a seperate fake timer for Innocents.
	/// The real timer is only displayed to Traitors as it increments every player death during the round.
	/// </summary>
	[Net]
	public TimeUntil FakeTime { get; private set; }
	public string FakeTimeFormatted => FakeTime.Relative.TimerString();

	public override string Name => "In Progress";
	public override int Duration => Game.InProgressTime;

	private int _innocentTeamDeathCount = 0;
	private readonly List<RoleButton> _logicButtons = new();

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		TimeLeft += Game.InProgressSecondsPerDeath;

		if ( player.Team == Team.Innocents )
			_innocentTeamDeathCount += 1;

		var percentDead = (float)_innocentTeamDeathCount / Team.Innocents.GetCount();
		if ( percentDead >= Game.CreditsAwardPercentage )
		{
			GivePlayersCredits( new Traitor(), Game.CreditsAwarded );
			_innocentTeamDeathCount = 0;
		}

		if ( player.Role is Traitor )
			GivePlayersCredits( new Detective(), Game.DetectiveTraitorDeathReward );
		else if ( player.Role is Detective && player.LastAttacker is Player p && p.IsAlive() && p.Team == Team.Traitors )
			GiveTraitorCredits( p );

		AlivePlayers.Remove( player );
		Spectators.Add( player );

		player.UpdateMissingInAction();
		ChangeRoundIfOver();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		Spectators.Add( player );
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

		Event.Run( TTTEvent.Round.RolesAssigned );

		if ( !Host.IsServer )
			return;

		FakeTime = TimeLeft;

		// If the map isn't armed for TTT, just give the player(s) a fixed loadout.
		if ( MapHandler.WeaponCount == 0 )
			foreach ( var player in AlivePlayers )
				GiveFixedLoadout( player );

		foreach ( var ent in Entity.All )
		{
			if ( ent is RoleButton button )
				_logicButtons.Add( button );
			else if ( ent is Corpse corpse )
				corpse.Delete();
		}
	}

	private static void GiveFixedLoadout( Player player )
	{
		if ( player.Inventory.Add( new MP5() ) )
			player.GiveAmmo( AmmoType.PistolSMG, 120 );

		if ( player.Inventory.Add( new Revolver() ) )
			player.GiveAmmo( AmmoType.Magnum, 20 );
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		PostRound.Load( Team.Innocents, WinType.TimeUp );
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

	public override void OnSecond()
	{
		if ( !Host.IsServer )
			return;

		if ( Game.PreventWin )
			TimeLeft += 1f;

		if ( TimeLeft )
			OnTimeUp();

		if ( !Utils.HasMinimumPlayers() && IsRoundOver() == Team.None )
			Game.Current.ForceStateChange( new WaitingState() );
	}

	private bool ChangeRoundIfOver()
	{
		var result = IsRoundOver();

		if ( result != Team.None && !Game.PreventWin )
		{
			PostRound.Load( result, WinType.Elimination );
			return true;
		}

		return false;
	}

	private static void GivePlayersCredits( Role role, int credits )
	{
		var clients = Utils.GetAliveClientsWithRole( role );

		clients.ForEach( ( cl ) =>
		{
			if ( cl.Pawn is Player p )
				p.Credits += credits;
		} );

		UI.InfoFeed.AddRoleEntry
		(
			To.Multiple( clients ),
			GameResource.GetInfo<RoleInfo>( role.Title ),
			$"You have been awarded {credits} credits for your performance."
		);
	}

	private static void GiveTraitorCredits( Player traitor )
	{
		traitor.Credits += Game.TraitorDetectiveKillReward;
		UI.InfoFeed.AddEntry( To.Single( traitor.Client ), $"have received {Game.TraitorDetectiveKillReward} credits for killing a Detective" );
	}

	[TTTEvent.Player.RoleChanged]
	private static void OnPlayerRoleChange( Player player, Role oldRole )
	{
		if ( Host.IsClient )
			return;

		if ( Game.Current.State is InProgress inProgress )
			inProgress.ChangeRoundIfOver();
	}
}
