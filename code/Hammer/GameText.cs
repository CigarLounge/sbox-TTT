using Sandbox;
using SandboxEditor;

namespace TTT;

public enum EntryType : byte
{
	All,
	Activator,
	Innocents,
	Traitors,
}

[ClassName( "ttt_game_text" )]
[Description( "Add text entry to the game feed when input fired." )]
[HammerEntity]
[Title( "Game Text" )]
public class GameText : Entity
{
	[Property]
	public string Message { get; set; } = "";

	[Description( "Who will this message go to?" )]
	[Property]
	public EntryType Receiver { get; set; } = EntryType.Activator;

	[Property]
	public Color Color { get; set; } = Color.White;

	[Input]
	public void DisplayMessage( Entity activator )
	{
		switch ( Receiver )
		{
			case EntryType.Activator:
				UI.InfoFeed.DisplayEntry( To.Single( activator ), Message, Color );
				break;

			case EntryType.All:
				UI.InfoFeed.DisplayEntry( To.Everyone, Message, Color );
				break;

			case EntryType.Innocents:
				UI.InfoFeed.DisplayEntry( Team.Innocents.ToClients(), Message, Color );
				break;

			case EntryType.Traitors:
				UI.InfoFeed.DisplayEntry( Team.Traitors.ToClients(), Message, Color );
				break;
		}
	}
}
