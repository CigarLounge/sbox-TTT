using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PossessionMeter : Panel
{
	private Panel PercentagePanel { get; set; }
	private readonly PropPossession _possession;

	public PossessionMeter( PropPossession possession )
	{
		_possession = possession;
		Local.Hud.AddChild( this );
	}

	public override void Tick()
	{
		var punchesFraction = (float)_possession.Punches / _possession.MaxPunches;
		PercentagePanel.Style.Width = Length.Fraction( punchesFraction );
	}
}