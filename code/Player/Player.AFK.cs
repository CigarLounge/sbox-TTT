using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class Player
{
	public static readonly List<InputButton> Buttons = Enum.GetValues( typeof( InputButton ) ).Cast<InputButton>().ToList();

	private TimeSince _timeSinceLastAction = 0f;

	private void CheckAFK()
	{
		if ( Client.IsBot )
			return;

		if ( IsForcedSpectator || !this.IsAlive() )
		{
			_timeSinceLastAction = 0;
			return;
		}

		var isAnyKeyPressed = Buttons.Any( Input.Down );
		var isMouseMoving = Input.MouseDelta != Vector2.Zero;

		if ( isAnyKeyPressed || isMouseMoving )
		{
			_timeSinceLastAction = 0f;
			return;
		}

		if ( _timeSinceLastAction > TTTGame.AFKTimer )
		{
			if ( TTTGame.KickAFKPlayers )
			{
				Log.Warning( $"Player ID: {Client.SteamId}, Name: {Client.Name} was kicked from the server for being AFK." );
				Client.Kick();
			}
			else
			{
				Log.Warning( $"Player ID: {Client.SteamId}, Name: {Client.Name} was moved to spectating for being AFK." );
				ToggleForcedSpectator();
			}
		}
	}
}
