using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTT.Events;
using TTT.Map;
using TTT.Player;
using TTT.Roles;

namespace TTT.Rounds;

public partial class InProgressRound : BaseRound
{
	public override string RoundName => "In Progress";

	[Net]
	public List<TTTPlayer> Players { get; set; }

	[Net]
	public List<TTTPlayer> Spectators { get; set; }

	private List<LogicButton> _logicButtons;

	public override int RoundDuration { get => Gamemode.Game.InProgressRoundTime; }

	public override void OnPlayerKilled( TTTPlayer player )
	{
		Players.Remove( player );
		Spectators.AddIfDoesNotContain( player );

		player.MakeSpectator();
		ChangeRoundIfOver();
	}

	public override void OnPlayerJoin( TTTPlayer player )
	{
		Spectators.AddIfDoesNotContain( player );
	}

	public override void OnPlayerLeave( TTTPlayer player )
	{
		Players.Remove( player );
		Spectators.Remove( player );

		ChangeRoundIfOver();
	}

	protected override void OnStart()
	{
		if ( !Host.IsServer )
			return;

		// For now, if the RandomWeaponCount of the map is zero, let's just give the players
		// a fixed weapon loadout.
		if ( MapHandler.RandomWeaponCount == 0 )
		{
			foreach ( TTTPlayer player in Players )
			{
				GiveFixedLoadout( player );
			}
		}

		// Cache buttons for OnSecond tick.
		_logicButtons = Entity.All.Where( x => x.GetType() == typeof( LogicButton ) ).Select( x => x as LogicButton ).ToList();
	}

	private static void GiveFixedLoadout( TTTPlayer player )
	{
		Log.Debug( $"Added Fixed Loadout to {player.Client.Name}" );
	}

	protected override void OnTimeUp()
	{
		LoadPostRound( Team.Innocents );

		base.OnTimeUp();
	}

	private Team IsRoundOver()
	{
		List<Team> aliveTeams = new();

		foreach ( TTTPlayer player in Players )
		{
			if ( !aliveTeams.Contains( player.Team ) )
			{
				aliveTeams.Add( player.Team );
			}
		}

		if ( aliveTeams.Count == 0 )
		{
			return Team.None;
		}

		return aliveTeams.Count == 1 ? aliveTeams[0] : Team.None;
	}

	public static void LoadPostRound( Team winningTeam )
	{
		Gamemode.Game.Current.MapSelection.TotalRoundsPlayed++;
		Gamemode.Game.Current.ForceRoundChange( new PostRound() );
		RPCs.ClientOpenAndSetPostRoundMenu(
			winningTeam.GetName(),
			winningTeam.GetColor()
		);
	}

	public override void OnSecond()
	{
		if ( Host.IsServer )
		{
			if ( !Gamemode.Game.PreventWin )
			{
				base.OnSecond();
			}
			else
			{
				TimeUntilRoundEnd += 1f;
			}

			_logicButtons.ForEach( x => x.OnSecond() ); // Tick role button delay timer.

			if ( !Utils.HasMinimumPlayers() && IsRoundOver() == Team.None )
			{
				Gamemode.Game.Current.ForceRoundChange( new WaitingRound() );
			}
		}
	}

	private bool ChangeRoundIfOver()
	{
		Team result = IsRoundOver();

		if ( result != Team.None && !Gamemode.Game.PreventWin )
		{
			LoadPostRound( result );

			return true;
		}

		return false;
	}

	[TTTEvent.Player.Role.Selected]
	private static void OnPlayerRoleChange( TTTPlayer player )
	{
		if ( Host.IsClient )
		{
			return;
		}

		if ( Gamemode.Game.Current.Round is InProgressRound inProgressRound )
		{
			inProgressRound.ChangeRoundIfOver();
		}
	}
}
