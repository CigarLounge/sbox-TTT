using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class C4DefuseMenu : Panel
{
	private readonly C4Entity _c4;
	private string _timer;
	private bool _isOwner => Game.LocalPawn == _c4.Owner;

	public C4DefuseMenu( C4Entity c4 ) => _c4 = c4;

	public override void Tick()
	{
		if ( _c4.IsArmed )
			_timer = TimeSpan.FromSeconds( _c4.TimeUntilExplode ).ToString( "mm':'ss" );
	}

	public void Pickup() => C4Entity.Pickup( _c4.NetworkIdent );
	public void Destroy() => C4Entity.DeleteC4( _c4.NetworkIdent );

	protected override int BuildHash() => HashCode.Combine( _timer, _c4.IsArmed, _isOwner );
}
