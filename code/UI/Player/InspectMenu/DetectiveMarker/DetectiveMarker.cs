using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class DetectiveMarker : Panel
{
	public readonly Vector3 CorpseLocation;
	private Label Distance { get; set; }
	private TimeSince _timeSinceCreation;

	public DetectiveMarker( Vector3 corpseLocation )
	{
		_timeSinceCreation = 0;
		CorpseLocation = corpseLocation;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		base.Tick();

		if ( _timeSinceCreation >= 30 )
		{
			Delete();
			return;
		}

		var player = Local.Pawn as Player;
		Distance.Text = $"{player.Position.Distance( CorpseLocation ).SourceUnitsToMeters():n0}m";

		var screenPos = CorpseLocation.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
