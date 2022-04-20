using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4Timer : WorldPanel
{
	private readonly C4Entity _c4;

	private Label Label { get; init; }

	public C4Timer( C4Entity c4 ) => _c4 = c4;

	public override void Tick()
	{
		base.Tick();

		SceneObject.Transform = _c4.GetAttachment( "timer" ) ?? default;

		if ( !_c4.IsArmed )
			Label.Text = "00:00";
		else
			Label.Text = _c4.TimeUntilExplode.Relative.TimerString();
	}
}
