using Editor;
using Sandbox;

namespace TTT;

[ClassName( "ttt_force_win" )]
[Description( "Forces round to end and win be awarded to team depending on input." )]
[HammerEntity]
[Title( "Force Win" )]
public class ForceWin : Entity
{
	[Description( "The team that will be forced to win." )]
	[Property]
	public Team Team { get; set; } = Team.Innocents;

	[Description( "OVERRIDES `Team` PROPERTY. When ActivateForceWin() is fired, this will award a win to the team of the activating player." )]
	[Property]
	public bool UseActivatorsTeam { get; set; }

	public override void Spawn()
	{
		Transmit = TransmitType.Never;
	}

	[Input]
	public void ActivateForceWin( Entity activator )
	{
		if ( TTTGame.Current.State is not InProgress )
			return;

		if ( UseActivatorsTeam )
		{
			if ( activator is Player player )
				PostRound.Load( player.Team, WinType.Objective );

			return;
		}

		PostRound.Load( Team, WinType.Objective );
	}
}
