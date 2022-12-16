using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class C4Hint : Panel
{
	private bool _hasDefuser = false;
	private readonly C4Entity _c4;

	public C4Hint( C4Entity c4 ) => _c4 = c4;

	public override void Tick()
	{
		_hasDefuser = Game.LocalPawn is Player player && player.ActiveCarriable is Defuser;
	}

	protected override int BuildHash() => HashCode.Combine( _c4.IsArmed, _hasDefuser );
}
