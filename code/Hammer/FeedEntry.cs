using Editor;
using Sandbox;

namespace TTT;

[ClassName( "ttt_feed_entry" )]
[Description( "Add text entry to the game feed when input fired." )]
[HammerEntity]
[Title( "Feed Entry" )]
public partial class FeedEntry : Entity
{
	[Net, Property]
	public string Text { get; private set; }

	[Description( "The team the message will be sent to. If set to `None`, the message will be sent to everyone." )]
	[Title( "Target Team" )]
	[Property]
	public Team Team { get; private set; } = Team.None;

	[Description( "OVERRIDES `Target Team` PROPERTY. When DisplayMessage() is fired, the message will only be sent to the activator." )]
	[Property]
	public bool ActivatorOnly { get; private set; }

	[Net, Property]
	public Color Color { get; private set; } = Color.White;

	[Input]
	public void DisplayEntry( Entity activator )
	{
		if ( ActivatorOnly )
		{
			if ( activator is Player player )
				DisplayEntry( To.Single( player ) );

			return;
		}

		if ( Team == Team.None )
			DisplayEntry( To.Everyone );
		else
			DisplayEntry( Team.ToClients() );
	}

	[ClientRpc]
	private void DisplayEntry()
	{
		UI.InfoFeed.AddEntry( Text, Color );
	}
}
