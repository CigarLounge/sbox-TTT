using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class Player
{
	public readonly static List<InputButton> Buttons = Enum.GetValues( typeof( InputButton ) ).Cast<InputButton>().ToList();

	private TimeSince _timeSinceLastAction = 0f;

	private void TickAFKSystem()
	{
		if ( Client.IsBot )
			return;

		if ( IsForcedSpectator || IsSpectator )
		{
			_timeSinceLastAction = 0;
			return;
		}

		bool isAnyKeyPressed = Buttons.Any( button => Input.Down( button ) );
		bool isMouseMoving = Input.MouseDelta != Vector3.Zero;

		if ( isAnyKeyPressed || isMouseMoving )
		{
			_timeSinceLastAction = 0f;
			return;
		}

		if ( _timeSinceLastAction > Game.AFKTimer )
		{
			if ( Game.KickAFKPlayers )
			{
				Log.Warning( $"Player ID: {Client.PlayerId}, Name: {Client.Name} was kicked from the server for being AFK." );
				Client.Kick();
			}
			else
			{
				Log.Warning( $"Player ID: {Client.PlayerId}, Name: {Client.Name} was moved to spectating for being AFK." );
				ToggleForcedSpectator();
			}
		}
	}
}
