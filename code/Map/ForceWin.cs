using Sandbox;
using SandboxEditor;

namespace TTT;

[Library( "ttt_force_win", Title = "Force Win", Description = "Forces round to end and win be awarded to team depending on input." ), HammerEntity]
public class ForceWin : Entity
{
	[Property( "Team", "The team that will be forced to win." )]
	public Team Team { get; set; }

	[Property( "Use Activators Team", "OVERRIDES `Team` PROPERTY. When ActivateForceWin() is fired, this will award a win to the team of the activating player." )]
	public bool UseActivatorsTeam { get; set; } = false;

	[Input]
	public void ActivateForceWin( Entity activator )
	{
		if ( Game.Current.State is not InProgress inProgress )
			return;

		if ( UseActivatorsTeam && activator is Player player )
			inProgress.LoadPostRound( player.Team, WinType.Objective );
		else
			inProgress.LoadPostRound( Team, WinType.Objective );
	}
}
