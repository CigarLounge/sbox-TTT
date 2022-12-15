using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionMeter : Panel
{
	private Panel PercentagePanel { get; init; }
	private readonly PropPossession _possession;

	public PossessionMeter( PropPossession possession )
	{
		_possession = possession;
		Game.RootPanel.AddChild( this );
	}

	public override void Tick()
	{
		var punchesFraction = (float)_possession.Punches / _possession.MaxPunches;
		PercentagePanel.Style.Width = Length.Fraction( punchesFraction );
	}
}
