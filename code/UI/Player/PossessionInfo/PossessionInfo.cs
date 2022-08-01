using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionInfo : Panel
{
	private Panel PercentageDiv { get; set; }

	public override void Tick()
	{
		if ( Local.Pawn is Player p && p.IsValid && p.PossessedProp is not null )
		{
			this.EnableFade( true );

			var punchesFraction = (float)p.PossessionPunches / Game.PropPossessionMaxPunches;
			PercentageDiv.Style.Width = Length.Fraction( punchesFraction );
			AddClass( "active" );
		}
		else
		{
			PercentageDiv.Style.Width = Length.Fraction( 0 );
			this.EnableFade( false );
			RemoveClass( "active" );
		}
	}
}
