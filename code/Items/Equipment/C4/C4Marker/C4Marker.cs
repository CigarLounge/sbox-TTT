using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class C4Marker : Panel
{
	private Vector3 _screenPos;
	private string _timer;
	private readonly C4Entity _c4;

	public C4Marker( C4Entity c4 )
	{
		_c4 = c4;
		WorldPoints.Instance.AddChild( this );
	}

	public override void Tick()
	{
		if ( !_c4.IsValid() || !_c4.IsArmed )
		{
			Delete();
			return;
		}

		_screenPos = _c4.Position.ToScreen();
		_timer = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
	}

	protected override int BuildHash() => HashCode.Combine( _timer, _screenPos );
}
