using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class RoundTimer : Panel
{
	private Label RoundName { get; init; }
	private Label Timer { get; init; }

	public override void Tick()
	{
		base.Tick();

		if ( Game.Current.Round is null )
			return;

		RoundName.Text = Game.Current.Round.RoundName;
		Timer.Text = Game.Current.Round is not WaitingRound ?
							$"{Game.Current.Round.TimeLeftFormatted}" :
							$"{Utils.MinimumPlayerCount()} / {Game.MinPlayers}";
	}
}
