using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class RoundTimer : Panel
{
	private readonly Label _currentRound;
	private readonly Label _currentTime;

	public RoundTimer()
	{
		StyleSheet.Load( "/ui/general/roundtimer/RoundTimer.scss" );

		AddClass( "background-color-primary" );
		AddClass( "opacity-heavy" );
		AddClass( "rounded" );
		AddClass( "text-shadow" );

		var roundWrapper = Add.Panel( "Round" );
		_currentRound = roundWrapper.Add.Label( "", "text-color-info" );

		var timerWrapper = Add.Panel( "Timer" );
		_currentTime = timerWrapper.Add.Label();
	}

	public override void Tick()
	{
		base.Tick();

		if ( Game.Current.Round == null )
			return;

		_currentRound.Text = Game.Current.Round.RoundName;
		_currentTime.Text = Game.Current?.Round is not WaitingRound ?
							$"{Game.Current?.Round?.TimeLeftFormatted}" :
							$"{Client.All.Count} / {Game.MinPlayers}";
	}
}
