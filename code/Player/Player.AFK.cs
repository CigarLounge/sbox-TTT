using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public partial class Player
{
	private static readonly List<InputButton> _buttons = Enum.GetValues( typeof( InputButton ) ).Cast<InputButton>().ToList();
	private TimeSince _timeSinceLastAction = 0f;

	private void CheckAFK()
	{
		if ( Client.IsBot )
			return;

		if ( !IsAlive )
		{
			_timeSinceLastAction = 0;
			return;
		}

		var isAnyKeyPressed = _buttons.Any( Input.Down );
		var isMouseMoving = Input.MouseDelta != Vector2.Zero;

		if ( isAnyKeyPressed || isMouseMoving )
		{
			_timeSinceLastAction = 0f;
			return;
		}

		if ( _timeSinceLastAction > GameManager.AFKTimer )
			Spectating.IsForced = true;
	}
}
