using Sandbox;
using Sandbox.UI;

[UseTemplate]
internal class ChatEntry : Panel
{
	public Label Name { get; set; }
	public Label Message { get; set; }

	private RealTimeSince _timeSinceCreation;

	public ChatEntry( string name, string message, Color? color = null )
	{
		_timeSinceCreation = 0;
		Name.Text = name;
		Message.Text = message;

		if ( color is not null )
			Name.Style.FontColor = color;
	}

	public override void Tick()
	{
		base.Tick();

		if ( _timeSinceCreation < 8 ) return;

		AddClass( "faded" );
	}
}

