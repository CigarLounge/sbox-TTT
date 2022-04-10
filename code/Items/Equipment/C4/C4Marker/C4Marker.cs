using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4Marker : Panel
{
	private readonly C4Entity _c4;

	private Label Timer { get; set; }

	public C4Marker( C4Entity c4 )
	{
		_c4 = c4;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !_c4.IsValid() )
		{
			Delete();
			return;
		}

		var screenPos = _c4.Position.ToScreen();
		this.Enabled( screenPos.z > 0f );

		if ( !this.IsEnabled() )
			return;

		Timer.Text = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
	}
}
