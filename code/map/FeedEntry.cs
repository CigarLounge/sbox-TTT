using Sandbox;

using TTT.Roles;

namespace TTT.Map;

public enum FeedEntryType
{
	All,
	Activator,
	Innocents,
	Traitors,
	Other
}

[Library( "ttt_feed_entry", Description = "Add text entry to the game feed when input fired." )]
public partial class FeedEntry : Entity
{
	[Property( "Message" )]
	public string Message { get; set; } = "";

	[Property( "Receiver", "Who will this message go to? If using a custom team, choose `Other` and set the `Receiver Team Override` to the name of your team." )]
	public FeedEntryType Receiver { get; set; } = FeedEntryType.Activator;

	[Property( "Text color" )]
	public Color Color { get; set; } = Color.White;

	[Property( "Receiver Team Override", "Use this ONLY if you're using a custom team name not listed in the `Receiver` list." )]
	public string ReceiverTeamOverride { get; set; } = "Override Team Name";

	public override void Spawn()
	{
		base.Spawn();

		Parent = Game.Current;
	}

	[Input]
	public void DisplayMessage( Entity activator )
	{
		switch ( Receiver )
		{
			case FeedEntryType.Activator:
				RPCs.ClientDisplayMessage( To.Single( activator ), Message, Color );

				break;

			case FeedEntryType.All:
				RPCs.ClientDisplayMessage( To.Everyone, Message, Color );

				break;

			case FeedEntryType.Innocents:
				RPCs.ClientDisplayMessage( Team.Innocents.ToClients(), Message, Color );

				break;

			case FeedEntryType.Traitors:
				RPCs.ClientDisplayMessage( Team.Traitors.ToClients(), Message, Color );

				break;

			case FeedEntryType.Other:
				TTTTeam team = TeamFunctions.TryGetTeam( ReceiverTeamOverride );

				if ( team != null )
				{
					RPCs.ClientDisplayMessage( To.Multiple( team.GetClients() ), Message, Color );
				}
				else
				{
					Log.Warning( $"Feed entry receiver value `{Receiver}` is incorrect and will not work." );
				}

				break;
		}
	}
}
