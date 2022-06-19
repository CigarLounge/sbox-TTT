using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_force_win" )]
[Description( "Forces round to end and win be awarded to team depending on input." )]
[HammerEntity]
[Title( "Force Win" )]
public class ForceWin : Entity
{
	[Description( "The team that will be forced to win." )]
	[Property]
	public Team Team { get; set; }

	[Description( "OVERRIDES `Team` PROPERTY. When ActivateForceWin() is fired, this will award a win to the team of the activating player." )]
	[Property]
	public bool UseActivatorsTeam { get; set; } = false;

	[Input]
	public void ActivateForceWin( Entity activator )
	{
		if ( Game.Current.State is not InProgress inProgress )
			return;

		if ( UseActivatorsTeam )
		{
			if ( activator is Player player )
				inProgress.LoadPostRound( player.Team, WinType.Objective );

			return;
		}

		inProgress.LoadPostRound( Team, WinType.Objective );
	}
}
