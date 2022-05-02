using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class DNAMarker : Panel
{
	public readonly Vector3 TargetPosition;

	private Label Distance { get; init; }

	public DNAMarker( Vector3 position )
	{
		TargetPosition = position;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;
		Distance.Text = $"{player.Position.Distance( TargetPosition ).SourceUnitsToMeters():n0}m";

		var screenPos = TargetPosition.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
