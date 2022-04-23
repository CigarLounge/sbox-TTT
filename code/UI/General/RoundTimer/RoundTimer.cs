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

		switch ( Game.Current.Round )
		{
			case WaitingRound:
			{
				Timer.Text = $"{Utils.MinimumPlayerCount()} / {Game.MinPlayers}";
				break;
			}

			case InProgressRound inProgressRound:
			{
				Timer.Text = inProgressRound.TimeUntilExpectedRoundEndFormatted;

				var isTeamTraitor = player.Team == Team.Traitors;
				if ( isTeamTraitor && inProgressRound.TimeUntilRoundEnd != inProgressRound.TimeUntilExpectedRoundEnd )
					SubText.Text = inProgressRound.TimeUntilRoundEndFormatted;

				if ( !isTeamTraitor && (int)inProgressRound.TimeUntilExpectedRoundEnd < 0 )
					SubText.Text = "OVERTIME";
				break;
			}

			default:
			{
				Timer.Text = Game.Current.Round.TimeUntilRoundEndFormatted;
				SubText.Text = string.Empty;
				break;
			}
		}
	}
}
