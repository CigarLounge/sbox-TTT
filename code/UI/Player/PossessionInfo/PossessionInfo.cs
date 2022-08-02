using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionInfo : Panel
{
	private Panel PercentageDiv { get; set; }

	private PropPossession _possession;

	public PossessionInfo( PropPossession possession )
	{
		_possession = possession;
		Local.Hud.AddChild( this );
	}

	public override void Tick()
	{
		var punchesFraction = (float)_possession.Punches / Game.PropPossessionMaxPunches;
		PercentageDiv.Style.Width = Length.Fraction( punchesFraction );
	}
}
