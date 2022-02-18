using Sandbox;

using TTT.Player;
using TTT.Rounds;
using TTT.Teams;

namespace TTT.Map;

[Library( "ttt_force_win", Description = "Forces round to end and win be awarded to team depending on input." )]
public partial class ForceWin : Entity
{
	[Property( "Team", "The name of the team that will be forced to win. This entity also contains built in inputs for certain teams. Use this for setting win conditions for custom teams." )]
	public string Team
	{
		get => _team;
		set
		{
			_team = value?.ToLower();
		}
	}

	[Property( "Use Activators Team", "OVERRIDES `Team` PROPERTY. When ForceWin() is fired, this will award a win to the team of the activating player." )]
	public bool UseActivatorsTeam { get; set; } = false;

	public override void Spawn()
	{
		base.Spawn();

		Parent = Game.Current;
	}

	[Input]
	public void InnocentsWin() => ForceEndRound( TeamFunctions.GetTeam( typeof( InnocentTeam ) ) );

	[Input]
	public void TraitorsWin() => ForceEndRound( TeamFunctions.GetTeam( typeof( TraitorTeam ) ) );

	[Input]
	public void ForceWin( Entity activator )
	{
		TTTTeam winningTeam;

		if ( UseActivatorsTeam && activator is TTTPlayer player )
		{
			winningTeam = player.Team;
		}
		else
		{
			winningTeam = TeamFunctions.TryGetTeam( Team );
		}

		if ( winningTeam != null )
		{
			ForceEndRound( winningTeam );

			return;
		}

		Log.Warning( $"ttt_force_win: Failed to grant win to team: {Team}, invalid or nonexistant team name." );
	}

	private static void ForceEndRound( TTTTeam team )
	{
		if ( Gamemode.Game.Current.Round is InProgressRound )
		{
			InProgressRound.LoadPostRound( team );
		}
	}
}
