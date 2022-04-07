using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class C4DefuseMenu : EntityHintPanel
{
	private readonly C4Entity _c4;

	public C4DefuseMenu( C4Entity c4 )
	{
		_c4 = c4;
	}
}
