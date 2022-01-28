using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Gamemode;

namespace TTT.UI
{
	public class GameTimerDisplay : Panel
	{
		private readonly Label _timerLabel;
		private readonly Label _roundLabel;

		public GameTimerDisplay()
		{
			StyleSheet.Load( "/ui/generalhud/gametimer/GameTimerDisplay.scss" );

			AddClass( "background-color-primary" );
			AddClass( "centered-horizontal" );
			AddClass( "opacity-heavy" );
			AddClass( "rounded" );
			AddClass( "text-shadow" );

			_roundLabel = Add.Label();
			_roundLabel.AddClass( "round-label" );
			_roundLabel.AddClass( "text-color-info" );

			_timerLabel = Add.Label();
			_timerLabel.AddClass( "timer-label" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Game.Instance.Round == null )
			{
				return;
			}

			_roundLabel.Text = $"{Game.Instance.Round.RoundName.ToUpper()}";

			_timerLabel.SetClass( "disabled", Game.Instance.Round is Rounds.WaitingRound );
			_timerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
		}
	}
}
