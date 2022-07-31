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

		if ( Game.Current.State is null )
			return;

		if ( Local.Pawn is not Player player )
			return;

		RoundName.Text = Game.Current.State.Name;

		if ( Game.Current.State is WaitingState )
			Timer.Text = $"{PlayerExtensions.MinimumPlayerCount()} / {Game.MinPlayers}";
		else
			Timer.Text = $"{Game.Current.State.TimeLeftFormatted}";

		if ( Game.Current.State is not InProgress inProgress )
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
