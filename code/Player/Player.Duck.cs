
using System.Threading;
using System.Numerics;
using Sandbox;
using System;

namespace TTT;


public partial class PlayerController : PawnController
{
	/// <summary> Is the player in a fully completed duck state? </summary>
	[Net, Predicted]
	public bool Ducked { get; private set; } = false;
	[Net, Predicted]
	public float DuckFraction { get; private set; } = 0f;

	[Net] public float DuckToggleSpeed { get; set; } = 7.0f;
	[Net] public float DuckHoldTime { get; set; } = 0.72f;

	[Net, Predicted]
	public TimeUntil DuckHoldUntil { get; private set; }


	public virtual void DuckPreTick()
	{
		var shouldDuck = Input.Down( InputButton.Duck );
		var onGround = GroundEntity != null;

		// Prevent cheap duck spamming exploit while airborne.
		if ( DuckHoldUntil > 0 )
		{
			if ( onGround ) DuckHoldUntil = 0;
			else shouldDuck = true;
		}

		// Toggle instantly if airborne.
		var duckDelta = onGround ? DuckToggleSpeed * Time.Delta : 999;

		if ( shouldDuck )
		{
			// Ducking.
			if ( !Ducked || DuckFraction < 1 )
			{
				DuckFraction = Math.Clamp( DuckFraction + duckDelta, 0, 1 );

				if ( DuckFraction.AlmostEqual( 1 ) )
					Duck();
			}
		}
		else
		{
			// Standing.
			if ( Ducked || DuckFraction > 0 )
			{
				// Stay ducked if there's no room to unduck.
				if ( !CanUnDuck() )
					DuckFraction = 1f;
				else
					DuckFraction = Math.Clamp( DuckFraction - duckDelta, 0, 1 );


				if ( DuckFraction.AlmostEqual( 0 ) )
					UnDuck();
			}
		}

		// Animation
		if ( Ducked )
			SetTag( "ducked" );
	}

	public void Duck()
	{
		if ( Ducked ) return;
		// Log.Info( $"[{Host.Name}] Ducked" );

		Ducked = true;
		DuckFraction = 1;

		UpdateBBox();

		// Tuck legs upward only if airborne.
		if ( GroundEntity == null )
		{
			DuckHoldUntil = DuckHoldTime;

			var distToCeil = TraceBBox( Position, Position + (Vector3.Up * DefaultHeight * 2f) ).Distance;
			var shift = MathF.Min( distToCeil, DuckHullOffset() );

			Position += Vector3.Up * shift;
			UpdateView( snapCamera: true );
		}
	}

	/// <summary>
	/// Is there something in the way of us unducking?
	/// </summary>
	public bool CanUnDuck()
	{
		var hull = GetHull();
		return !TraceBBox( Position, Position, hull.Mins, hull.Maxs ).StartedSolid;
	}

	public void UnDuck()
	{
		if ( !Ducked ) return;
		// Log.Info( $"[{Host.Name}] Un-Ducked" );

		if ( !CanUnDuck() )
			return;

		Ducked = false;
		DuckFraction = 0;

		UpdateBBox();

		// Extend legs downward only if airborne.
		if ( GroundEntity == null )
		{
			var distToGround = TraceBBox( Position, Position + Vector3.Down * DefaultHeight * 2f ).Distance;
			var shift = MathF.Min( DuckHullOffset(), distToGround );

			Position += Vector3.Down * shift;
			UpdateView( snapCamera: true );
		}
	}

	public float ActiveEyeHeight()
	{
		var eyeOffset = DefaultHeight - EyeHeight;
		var zStand = DefaultHeight - eyeOffset;
		var zDuck = DuckHeight - eyeOffset;

		return zDuck + ((zStand - zDuck) * (1 - DuckFraction));
	}

	public float DuckHullOffset()
	{
		return MathF.Max( DefaultHeight - DuckHeight, 0f );
	}

	Vector3 _mins = Vector3.Zero;
	Vector3 _maxs = Vector3.Zero;

	public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
	{
		_mins = mins;
		_maxs = maxs;

		if ( Ducked )
			maxs = maxs.WithZ( DuckHeight * scale );
	}

}
