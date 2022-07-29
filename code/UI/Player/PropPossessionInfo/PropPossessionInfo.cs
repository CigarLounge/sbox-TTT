using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PropPossessionInfo : Panel
{
	private Panel PercentageDiv { get; set; }

	public override void Tick()
	{
		if ( Local.Pawn is Player p && p.IsValid && p.PossessedProp is not null )
		{
			this.EnableFade( true );

			var punchesFraction = (float)p.PossessionPunches / Game.PropPossessionMaxPunches;
			PercentageDiv.Style.Width = Length.Fraction( punchesFraction );
		}
		else
		{
			PercentageDiv.Style.Width = Length.Fraction( 1 );
			this.EnableFade( false );
		}
	}
}
