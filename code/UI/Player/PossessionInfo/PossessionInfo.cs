using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionInfo : Panel
{
	private Panel PercentagePanel { get; init; }

	private PropPossession _possession;

	public PossessionInfo( PropPossession possession )
	{
		_possession = possession;
		Local.Hud.AddChild( this );
	}

	public override void Tick()
	{
		var punchesFraction = (float)_possession.Punches / Game.PropPossessionMaxPunches;
		PercentagePanel.Style.Width = Length.Fraction( punchesFraction );
	}
}
