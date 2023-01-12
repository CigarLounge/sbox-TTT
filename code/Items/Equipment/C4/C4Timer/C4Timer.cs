using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class C4Timer : WorldPanel
{
	private readonly C4Entity _c4;
	public C4Timer( C4Entity c4 ) => _c4 = c4;
	public override void Tick() => SceneObject.Transform = _c4.GetAttachment( "timer" ) ?? default;
	protected override int BuildHash() => HashCode.Combine( _c4.IsArmed, _c4.TimeUntilExplode.Relative.TimerFormat() );
}
