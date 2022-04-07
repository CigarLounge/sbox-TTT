using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4Hint : EntityHintPanel
{
	private readonly C4Entity _c4;
	private Label SubText { get; set; }

	public C4Hint( C4Entity c4 )
	{
		_c4 = c4;
	}

	public override void Tick()
	{
		SubText.Text = _c4.IsArmed ? "to defuse." : "to arm.";
	}
}
