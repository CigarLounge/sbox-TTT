using Sandbox.UI;

namespace TTT.UI;

public class HintDisplay : Panel
{
	public static HintDisplay Instance { get; set; }

	public HintDisplay() : base()
	{
		Instance = this;

		AddClass( "fullscreen" );
	}
}
