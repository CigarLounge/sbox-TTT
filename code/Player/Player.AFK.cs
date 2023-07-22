using Sandbox;

namespace TTT;

public partial class Player
{
	private TimeSince _timeSinceLastAction = 0f;

	private void CheckAFK()
	{
		if ( Client.IsBot || Spectating.IsForced )
			return;

		if ( !IsAlive )
		{
			_timeSinceLastAction = 0;
			return;
		}

		var isAnyKeyPressed = false;

		foreach ( var action in InputAction.All )
			isAnyKeyPressed |= Input.Down( action );

		var isMouseMoving = Input.MouseDelta != Vector2.Zero;

		if ( isAnyKeyPressed || isMouseMoving )
		{
			_timeSinceLastAction = 0f;
			return;
		}

		if ( _timeSinceLastAction > GameManager.AFKTimer )
		{
			Spectating.IsForced = true;
			Input.StopProcessing = true;
		}
	}
}
