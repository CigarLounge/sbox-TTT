using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class C4ArmMenu : Panel
{
	public int Timer { get; set; } = 45;

	private readonly C4Entity _c4;

	public C4ArmMenu( C4Entity c4 ) => _c4 = c4;

	public void Arm() => C4Entity.ArmC4( _c4.NetworkIdent, Timer );
	public void Pickup() => C4Entity.Pickup( _c4.NetworkIdent );
	public void Destroy() => C4Entity.DeleteC4( _c4.NetworkIdent );
	protected override int BuildHash() => HashCode.Combine( Timer );
}
