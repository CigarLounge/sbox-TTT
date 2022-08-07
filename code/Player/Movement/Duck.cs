using System;
using Sandbox;

namespace TTT;

[Library]
public class Duck : BaseNetworkable
{
	public BasePlayerController Controller;
	public bool IsActive;

	private Vector3 _originalMins;
	private Vector3 _originalMaxs;

	public Duck( BasePlayerController controller )
	{
		Controller = controller;
	}

	public void PreTick()
	{
		var wants = Input.Down( InputButton.Duck );

		if ( wants != IsActive )
		{
			if ( wants )
				TryDuck();
			else
				TryUnDuck();
		}

		if ( IsActive )
			Controller.SetTag( "ducked" );
	}

	protected void TryDuck()
	{
		var wasactive = IsActive;
		IsActive = true;

		if ( !wasactive && IsActive )
			Controller.Position += Vector3.Up * 14;
	}

	protected void TryUnDuck()
	{
		var wasactive = IsActive;

		var pm = Controller.TraceBBox( Controller.Position, Controller.Position, _originalMins, _originalMaxs );
		if ( pm.StartedSolid )
			return;

		IsActive = false;

		if ( wasactive && !IsActive && Controller.GroundEntity == null )
		{
			var distToGround = Controller.TraceBBox( Controller.Position, Controller.Position + Vector3.Down * 1000 ).Distance;
			var shift = MathF.Min( 14, distToGround );
			Controller.Position += Vector3.Down * shift;
		}
	}

	public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
	{
		_originalMins = mins;
		_originalMaxs = maxs;

		if ( IsActive )
			maxs = maxs.WithZ( 36 * scale );
	}

	public float GetWishSpeed()
	{
		if ( !IsActive )
			return -1;
		return 88;
	}
}
