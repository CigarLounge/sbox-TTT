
using System.Threading;
using System.Numerics;
using Sandbox;
using System;

namespace TTT;


[Library]
public partial class PlayerDuck : BaseNetworkable
{
	public PlayerController Walk;

	/// <summary> Is the player in a fully completed duck state? </summary>
	// [Net]
	// [Predicted]
	[Net, Predicted]
	public bool Ducked { get; private set; } = false;
	// [Net]
	// [Predicted]
	[Net, Predicted]
	public float Fraction { get; private set; } = 0f;
	[Net]
	public float ToggleSpeed { get; set; } = 7.0f;

	[Net]
	public float StuckDelay { get; set; } = 0.72f;
	// [Predicted]
	[Net, Predicted]
	public TimeUntil StuckUntil { get; private set; }


	public PlayerDuck( PlayerController controller )
	{
		Walk = controller;
	}

	public virtual void PreTick()
	{
		var shouldDuck = Input.Down( InputButton.Duck );
		var onGround = Walk.GroundEntity != null;

		// Prevent cheap duck spamming exploit in the air.
		if (StuckUntil > 0)
		{
			Log.Info( $"[{Host.Name}] StuckUntil: {StuckUntil}" );
			if (onGround) StuckUntil = 0;
			else shouldDuck = true;
		}

		// Toggle instantly if airborne.
		var duckDelta = onGround ? ToggleSpeed * Time.Delta : 999;

		if ( shouldDuck )
		{
			// Ducking.
			if ( !Ducked || Fraction < 1 )
			{
				// if ( Prediction.FirstTime )
				if ( Prediction.FirstTime && Host.IsClient )
					Log.Info( $"[{Host.Name}] FractionBefore: {Fraction}" );

				Fraction = Math.Clamp( Fraction + duckDelta, 0, 1 );

				// if ( Prediction.FirstTime )
				if ( Prediction.FirstTime && Host.IsClient )
					Log.Info( $"[{Host.Name}] FractionAfter: {Fraction}" );

				if ( Fraction.AlmostEqual( 1 ) )
					Duck();
			}
		}
		else
		{
			// Standing.
			if ( Ducked || Fraction > 0 )
			{
				// if ( Prediction.FirstTime )
				if ( Prediction.FirstTime && Host.IsClient )
					Log.Info( $"[{Host.Name}] FractionBefore: {Fraction}" );

				// TODO: Reapply duck if there's no room to unduck.
				Fraction = Math.Clamp( Fraction - duckDelta, 0, 1 );

				// if ( Prediction.FirstTime )
				if ( Prediction.FirstTime && Host.IsClient )
					Log.Info( $"[{Host.Name}] FractionAfter: {Fraction}" );

				if ( Fraction.AlmostEqual( 0 ) )
					UnDuck();
			}
		}

		// Animation
		if ( Ducked )
			Walk.SetTag( "ducked" );
	}

	public bool InProgress()
	{
		return (Ducked && Fraction < 1f) || (!Ducked && Fraction > 0f);
	}

	public void Duck()
	{
		if ( Ducked ) return;
		Log.Info( $"[{Host.Name}] Ducked" );

		Ducked = true;
		Fraction = 1;

		Walk.UpdateBBox();

		// Tuck legs upward only if airborne.
		if ( Walk.GroundEntity == null )
		{
			StuckUntil = StuckDelay;
			Log.Info( $"[{Host.Name}] Skill ceiling lowered: {StuckUntil}" );

			var distToCeil = Walk.TraceBBox( Walk.Position, Walk.Position + (Vector3.Up * Walk.BodyHeight * 2f) ).Distance;
			var shift = MathF.Min( distToCeil, DuckHullOffset() );

			Walk.Position += Vector3.Up * shift;
			Walk.UpdateView( snapCamera: true );
		}
	}

	public void UnDuck()
	{
		if ( !Ducked ) return;
		Log.Info( $"[{Host.Name}] Un-Ducked" );

		// Don't unduck if stuck.
		var pm = Walk.TraceBBox( Walk.Position, Walk.Position, _mins, _maxs );
		if ( pm.StartedSolid ) return;

		Ducked = false;
		Fraction = 0;

		Walk.UpdateBBox();

		// Extend legs downward only if airborne.
		if ( Walk.GroundEntity == null )
		{
			var distToGround = Walk.TraceBBox( Walk.Position, Walk.Position + Vector3.Down * Walk.BodyHeight * 2f ).Distance;
			var shift = MathF.Min( DuckHullOffset(), distToGround );

			Walk.Position += Vector3.Down * shift;
			Walk.UpdateView( snapCamera: true );
		}
	}

	public virtual float GetWishSpeed()
	{
		if ( !Ducked ) return -1f;
		return 93f;
	}

	public float EyeHeight()
	{
		var eyeOffset = Walk.BodyHeight - Walk.EyeHeight;
		var zStand = Walk.BodyHeight - eyeOffset;
		var zDuck = Walk.DuckHeight - eyeOffset;

		// No built-in lerp method. What the fuck?
		// var frac = Ducked ? 1 : 0;
		// return zDuck + ((zStand - zDuck) * (1 - frac));
		return zDuck + ((zStand - zDuck) * (1 - Fraction));
	}

	Vector3 _mins = Vector3.Zero;
	Vector3 _maxs = Vector3.Zero;

	public virtual void UpdateBBox( ref Vector3 mins, ref Vector3 maxs, float scale )
	{
		_mins = mins;
		_maxs = maxs;

		if ( Ducked )
			maxs = maxs.WithZ( Walk.DuckHeight * scale );
	}

	public float DuckHullOffset()
	{
		return MathF.Max( Walk.BodyHeight - Walk.DuckHeight, 0f );
	}

}
