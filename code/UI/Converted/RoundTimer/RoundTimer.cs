using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class RoundTimer : Panel
{
	private Label RoundName { get; set; }
	private Label Timer { get; set; }
	private Label SubText { get; set; }

	public override void Tick()
	{
		base.Tick();

		if ( Game.Current.State is null || Local.Pawn is not Player player )
			return;

		RoundName.Text = Game.Current.State.Name;

		if ( Game.Current.State is WaitingState )
			Timer.Text = $"{Utils.MinimumPlayerCount()} / {Game.MinPlayers}";
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
