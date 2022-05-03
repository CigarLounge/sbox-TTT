using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class DNAMarker : Panel
{
	private Label Distance { get; init; }
	private readonly Vector3 _targetPosition;

	public DNAMarker( Vector3 position )
	{
		_targetPosition = position;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;
		Distance.Text = $"{player.Position.Distance( _targetPosition ).SourceUnitsToMeters():n0}m";

		var screenPos = _targetPosition.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
