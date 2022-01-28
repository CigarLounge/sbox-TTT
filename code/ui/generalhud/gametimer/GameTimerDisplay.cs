using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Gamemode;

namespace TTT.UI
{
	public class GameTimerDisplay : Panel
	{
		private readonly Panel _timerPanel;
		private readonly Label _timerLabel;
		private readonly Panel _roundPanel;
		private readonly Label _roundLabel;

		public GameTimerDisplay() : base()
		{
			StyleSheet.Load( "/ui/generalhud/gametimer/GameTimerDisplay.scss" );

			AddClass( "background-color-primary" );
			AddClass( "centered-horizontal" );
			AddClass( "opacity-heavy" );
			AddClass( "rounded" );
			AddClass( "text-shadow" );

			_timerPanel = new( this );
			_timerPanel.AddClass( "timer-panel" );

			_timerLabel = _timerPanel.Add.Label();
			_timerLabel.AddClass( "timer-label" );

			_roundPanel = new( this );
			_roundPanel.AddClass( "round-panel" );

			_roundLabel = _roundPanel.Add.Label();
			_roundLabel.AddClass( "round-label" );
			_roundLabel.AddClass( "text-color-info" );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Game.Instance.Round == null )
			{
				return;
			}

			_roundLabel.Text = $"{Game.Instance.Round.RoundName.ToUpper()}";

			_timerPanel.SetClass( "disabled", Game.Instance.Round is Rounds.WaitingRound );
			_timerLabel.Text = Game.Instance.Round.TimeLeftFormatted;
		}
	}
}
