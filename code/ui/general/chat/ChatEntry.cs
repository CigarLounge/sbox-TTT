using Sandbox;
using Sandbox.UI;

[UseTemplate]
internal class ChatEntry : Panel
{
	public Label Name { get; set; }
	public Label Message { get; set; }

	private TimeSince timeSinceCreation;

	public ChatEntry( string name, string message, Color? color = null )
	{
		timeSinceCreation = 0;
		Name.Text = name;
		Message.Text = message;

		if ( color != null )
			Name.Style.FontColor = color;
	}

	public override void Tick()
	{
		base.Tick();

		if ( timeSinceCreation < 8 ) return;

		AddClass( "faded" );
	}
}

