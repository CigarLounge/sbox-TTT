using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4ArmMenu : EntityHintPanel
{
	private readonly C4Entity _c4;

	public C4ArmMenu() { }

	public C4ArmMenu( C4Entity c4 ) => _c4 = c4;
}
