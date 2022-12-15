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

		if ( GameManager.Current.State is null )
			return;

		if ( Game.LocalPawn is not Player player )
			return;

		RoundName.Text = GameManager.Current.State.Name;

		if ( GameManager.Current.State is WaitingState )
			Timer.Text = $"{Utils.MinimumPlayerCount()} / {GameManager.MinPlayers}";
		else
			Timer.Text = $"{GameManager.Current.State.TimeLeftFormatted}";

		if ( GameManager.Current.State is not InProgress inProgress )
		{
			SubText.SetClass( "show", false );
			return;
		}

		Timer.Text = inProgress.FakeTimeFormatted;
		var isTraitor = player.Team == Team.Traitors;

		if ( isTraitor && inProgress.TimeLeft != inProgress.FakeTime )
		{
			SubText.Text = inProgress.TimeLeftFormatted;
			SubText.SetClass( "show", true );
		}
		else if ( !isTraitor && (int)inProgress.FakeTime < 0 )
		{
			SubText.Text = "OVERTIME";
			SubText.SetClass( "show", true );
		}
	}
}
