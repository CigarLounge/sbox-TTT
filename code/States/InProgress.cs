using Sandbox;
using System.Collections.Generic;
using System.Linq;

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

	public override string Name { get; } = "In Progress";
	public override int Duration => GameManager.InProgressTime;

	private int _innocentTeamDeathCount = 0;
	private readonly List<RoleButton> _logicButtons = new();

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		TimeLeft += GameManager.InProgressSecondsPerDeath;

		if ( player.Team == Team.Innocents )
			_innocentTeamDeathCount += 1;

		var percentDead = (float)_innocentTeamDeathCount / Team.Innocents.GetCount();
		if ( percentDead >= GameManager.CreditsAwardPercentage )
		{
			GivePlayersCredits<Traitor>( GameManager.CreditsAwarded );
			_innocentTeamDeathCount = 0;
		}

		if ( player.Role is Traitor )
			GivePlayersCredits<Detective>( GameManager.DetectiveTraitorDeathReward );
		else if ( player.Role is Detective && player.LastAttacker is Player p && p.IsAlive() && p.Team == Team.Traitors )
			GiveTraitorCredits( p );

		AlivePlayers.Remove( player );
		Spectators.Add( player );

		player.UpdateMissingInAction();
		CheckForResult();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		player.Status = PlayerStatus.Spectator;
		player.UpdateStatus( To.Everyone );
		Spectators.Add( player );
	}

	public override void OnPlayerLeave( Player player )
	{
		base.OnPlayerLeave( player );

		AlivePlayers.Remove( player );
		Spectators.Remove( player );

		CheckForResult();
	}

	protected override void OnStart()
	{
		Event.Run( GameEvent.Round.Start );

		if ( !Game.IsServer )
			return;

		FakeTime = TimeLeft;

		// If the map isn't armed for TTT, just give the player(s) a fixed loadout.
		if ( MapHandler.WeaponCount == 0 )
		{
			foreach ( var player in AlivePlayers )
				GiveFixedLoadout( player );
		}

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
		PostRound.Load( Team.Innocents, WinType.TimeUp );
	}

	public override void OnSecond()
	{
		if ( !Game.IsServer )
			return;

		if ( GameManager.PreventWin )
			TimeLeft += 1f;

		if ( TimeLeft )
			OnTimeUp();
	}

	private void CheckForResult()
	{
		HashSet<Team> aliveTeams = new();
		foreach ( var player in AlivePlayers )
			aliveTeams.Add( player.Team );

		if ( aliveTeams.Count == 0 )
			PostRound.Load( Team.None, WinType.Elimination );
		else if ( aliveTeams.Count == 1 )
			PostRound.Load( aliveTeams.FirstOrDefault(), WinType.Elimination );
	}

	private static void GivePlayersCredits<T>( int credits ) where T : Role
	{
		var clients = Utils.GetClientsWhere( p => p.IsAlive() && p.Role is T );
		clients.ForEach( c => (c.Pawn as Player).Credits += credits );

		UI.InfoFeed.AddRoleEntry
		(
			To.Multiple( clients ),
			GameResource.GetInfo<RoleInfo>( typeof( T ) ),
			$"You have been awarded {credits} credits for your performance."
		);
	}

	private static void GiveTraitorCredits( Player traitor )
	{
		traitor.Credits += GameManager.TraitorDetectiveKillReward;
		UI.InfoFeed.AddEntry( To.Single( traitor.Client ), traitor, $"have received {GameManager.TraitorDetectiveKillReward} credits for killing a Detective" );
	}

	[GameEvent.Player.RoleChanged]
	private static void OnPlayerRoleChange( Player player, Role oldRole )
	{
		if ( !Game.IsServer )
			return;

		if ( GameManager.Current.State is InProgress inProgress )
			inProgress.CheckForResult();
	}
}
