using Sandbox;

namespace TTT;

public enum EntryType : byte
{
	All,
	Activator,
	Innocents,
	Traitors,
}

[Library( "ttt_game_text", Description = "Add text entry to the game feed when input fired." )]
public partial class GameText : Entity
{
	[Property( "Message" )]
	public string Message { get; set; } = "";

	[Property( "Receiver", "Who will this message go to?" )]
	public EntryType Receiver { get; set; } = EntryType.Activator;

	[Property( "Text color" )]
	public Color Color { get; set; } = Color.White;

	[Input]
	public void DisplayMessage( Entity activator )
	{
		switch ( Receiver )
		{
			case EntryType.Activator:
				RPCs.ClientDisplayEntry( To.Single( activator ), Message, Color );
				break;

			case EntryType.All:
				RPCs.ClientDisplayEntry( To.Everyone, Message, Color );
				break;

			case EntryType.Innocents:
				RPCs.ClientDisplayEntry( Team.Innocents.ToClients(), Message, Color );
				break;

			case EntryType.Traitors:
				RPCs.ClientDisplayEntry( Team.Traitors.ToClients(), Message, Color );
				break;
		}
	}
}
