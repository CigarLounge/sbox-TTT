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

		RoundName.Text = Game.Current.Round.Name;

		if ( Game.Current.Round is WaitingState )
			Timer.Text = $"{Utils.MinimumPlayerCount()} / {Game.MinPlayers}";
		else
			Timer.Text = $"{Game.Current.Round.TimeLeftFormatted}";

		if ( Game.Current.Round is not InProgress inProgressRound )
		{
			SubText.SetClass( "show", false );
			return;
		}

		Timer.Text = inProgressRound.FakeTimeFormatted;
		bool isTraitor = player.Team == Team.Traitors;

		if ( isTraitor && inProgressRound.TimeLeft != inProgressRound.FakeTime )
		{
			SubText.Text = inProgressRound.TimeLeftFormatted;
			SubText.SetClass( "show", true );
		}
		else if ( !isTraitor && (int)inProgressRound.FakeTime < 0 )
		{
			SubText.Text = "OVERTIME";
			SubText.SetClass( "show", true );
		}
	}
}
