using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class Wire : Panel
{
	public Label Number { get; set; }
	public Panel WireDisplay { get; set; }

	public void Cut()
	{
		WireDisplay.AddClass( "cut-wire" );
	}
}
