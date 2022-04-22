using Sandbox;

namespace TTT;

[Library( "ttt_force_win", Description = "Forces round to end and win be awarded to team depending on input." )]
public class ForceWin : Entity
{
	[Property( "Team", "The name of the team that will be forced to win." )]
	public Team Team { get; set; }

	[Property( "Use Activators Team", "OVERRIDES `Team` PROPERTY. When ActivateForceWin() is fired, this will award a win to the team of the activating player." )]
	public bool UseActivatorsTeam { get; set; } = false;

	[Input]
	public void ActivateForceWin( Entity activator )
	{
		if ( UseActivatorsTeam && activator is Player player )
		{
			ForceEndRound( player.Team );
		}
		else
		{
			ForceEndRound( Team );
		}
	}

	private static void ForceEndRound( Team team )
	{
		if ( Game.Current.Round is InProgressRound inProgressRound )
			inProgressRound.LoadPostRound( team );
	}
}
