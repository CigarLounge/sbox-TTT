using System;
using Sandbox;

namespace TTT;

public class Duck : BaseNetworkable
{
	public WalkController Controller;
	public bool IsActive;

	private Vector3 _originalMins;
	private Vector3 _originalMaxs;

	public Duck( WalkController controller )
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
		{
			Controller.SetTag( "ducked" );

			var positionScale = 0.5f;
			var lookDownDot = Controller.Player.EyeRotation.Forward.Dot( Vector3.Down );

			if ( lookDownDot > 0.5f )
			{
				var f = lookDownDot - 0.5f;
				positionScale += f.Remap( 0f, 0.5f, 0f, 0.1f );
			}

			Controller.Player.EyeLocalPosition *= positionScale;
		}
	}

	public void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
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

	private void TryDuck()
	{
		IsActive = true;
	}

	private void TryUnDuck()
	{
		var pm = Controller.TraceBBox( Controller.Player.Position, Controller.Player.Position, _originalMins, _originalMaxs );
		if ( pm.StartedSolid )
			return;

		IsActive = false;
	}
}
