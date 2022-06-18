using Sandbox;
using SandboxEditor;

namespace TTT;

[ClassName( "ttt_game_text" )]
[Description( "Add text entry to the game feed when input fired." )]
[HammerEntity]
[Title( "Game Text" )]
public partial class GameText : Entity
{
	[Description( "The team that will be forced to win. If set to `None`, the message will be sent to everyone." )]
	[Title( "Target Team" )]
	[Property]
	public Team Team { get; private set; } = Team.None;

	[Net, Property]
	public string Message { get; private set; }

	[Description( "OVERRIDES `Target Team` PROPERTY. When DisplayMessage() is fired, the message will only be sent to the activator's team." )]
	[Property]
	public bool UseActivatorsTeam { get; private set; }

	[Net, Property]
	public Color Color { get; private set; } = Color.White;

	[Input]
	public void DisplayMessage( Entity activator )
	{
		if ( UseActivatorsTeam )
		{
			if ( activator is Player player )
				DisplayMessage( player.Team.ToClients() );

			return;
		}

		if ( Team == Team.None )
			DisplayMessage( To.Everyone );
		else
			DisplayMessage( Team.ToClients() );
	}

	[ClientRpc]
	private void DisplayMessage()
	{
		UI.InfoFeed.DisplayEntry( Message, Color );
	}
}
