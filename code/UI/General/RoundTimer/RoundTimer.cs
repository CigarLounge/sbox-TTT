using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class RoundTimer : Panel
{
	private Label RoundName { get; init; }
	private Label Timer { get; init; }
	private Label SubText { get; init; }

	public override void Tick()
	{
		base.Tick();

		if ( Game.Current.Round is null || Local.Pawn is not Player player )
			return;

		RoundName.Text = Game.Current.Round.RoundName;
		Timer.Text = Game.Current.Round is not WaitingRound ?
							$"{Game.Current.Round.TimeUntilRoundEndFormatted}" :
							$"{Utils.MinimumPlayerCount()} / {Game.MinPlayers}";

		if ( Game.Current.Round is InProgressRound inProgressRound )
		{
			var isTeamTraitor = player.Team == Team.Traitors;
			if ( isTeamTraitor && inProgressRound.TimeUntilRoundEnd != inProgressRound.TimeUntilActualRoundEnd )
			{
				SubText.Text = inProgressRound.TimeUntilActualRoundEndFormatted;
				SubText.SetClass( "show", true );
			}

			if ( !isTeamTraitor && (int)inProgressRound.TimeUntilRoundEnd < 0 )
			{
				SubText.Text = "OVERTIME";
				SubText.SetClass( "show", true );
			}

			return;
		}

		SubText.SetClass( "show", false );
	}
}
