using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TextChatEntry : Panel
{
	public string Name;
	public string Message;
	public Color? NameColor;

	private RealTimeSince _timeSinceCreation = 0;

	public override void Tick()
	{
		if ( _timeSinceCreation < 8 )
			return;

		AddClass( "faded" );
	}
}

