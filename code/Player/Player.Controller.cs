
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TTT;


public partial class PlayerController : PawnController
{
	[Net] public float WalkSpeed { get; set; } = 110.0f;
	[Net] public float DefaultSpeed { get; set; } = 237.0f;
	[Net] public float Acceleration { get; set; } = 10.0f;
	[Net] public float AirAcceleration { get; set; } = 50.0f;
	[Net] public float FallSoundZ { get; set; } = -30.0f;
	[Net] public float GroundFriction { get; set; } = 4.0f;
	[Net] public float StopSpeed { get; set; } = 150.0f;
	[Net] public float Size { get; set; } = 20.0f;
	[Net] public float DistEpsilon { get; set; } = 0.03125f;
	[Net] public float GroundAngle { get; set; } = 46.0f;
	[Net] public float Bounce { get; set; } = 0.0f;
	[Net] public float MoveFriction { get; set; } = 1.0f;
	[Net] public float StepSize { get; set; } = 18.0f;
	[Net] public float MaxNonJumpVelocity { get; set; } = 140.0f;
	[Net] public float BodyGirth { get; set; } = 32.0f;
	[Net] public float BodyHeight { get; set; } = 72.0f;
	[Net] public float EyeHeight { get; set; } = 64.0f;
	[Net] public float Gravity { get; set; } = 800.0f;
	[Net] public float AirControl { get; set; } = 30.0f;
	public bool Swimming { get; set; } = false;
	[Net] public bool AutoJump { get; set; } = false;

	[Net, Change]
	public PlayerDuck Duck { get; private init; }
	public Unstuck Unstuck;

	[Net, Predicted]
	public bool Momentum { get; set; }

	[Net] public float DuckHeight { get; set; } = 36f;
	[Net] public float JumpSpeed { get; set; } = 180f;

	private Vector3 _lastBaseVelocity;

	private const float FallDamageThreshold = 650f;
	private const float FallDamageScale = 0.33f;


	public PlayerController()
	{
		Duck = new PlayerDuck( this );
	}

	// public void OnDeactivate();
	// public void OnActivate();

	public override void Simulate()
	{
		_lastBaseVelocity = BaseVelocity;

		ApplyMomentum();

		BaseSimulate();

		// Check for fall damage.
		var fallVelocity = -Pawn.Velocity.z;
		if ( GroundEntity is null || fallVelocity <= 0 )
			return;

		if ( fallVelocity > FallDamageThreshold )
		{
			var damage = (MathF.Abs( fallVelocity ) - FallDamageThreshold) * FallDamageScale;

			if ( Host.IsServer )
			{
				Pawn.TakeDamage( new DamageInfo
				{
					Attacker = Pawn,
					Flags = DamageFlags.Fall,
					Force = Vector3.Down * fallVelocity,
					Damage = damage,
				} );
			}

			Pawn.PlaySound( Strings.FallDamageSound ).SetVolume( (damage * 0.05f).Clamp( 0, 0.5f ) );
		}
	}

	public virtual void AirMove()
	{
		SurfaceFriction = 1f;

		var wishdir = WishVelocity.Normal;
		var wishspeed = WishVelocity.Length;

		Accelerate( wishdir, wishspeed, AirControl, AirAcceleration );

		Velocity += BaseVelocity;

		Move();

		Velocity -= BaseVelocity;
	}

	public virtual void CategorizePosition( bool bStayOnGround )
	{
		SurfaceFriction = 1.0f;

		// Doing this before we move may introduce a potential latency in water detection, but
		// doing it after can get us stuck on the bottom in water if the amount we move up
		// is less than the 1 pixel 'threshold' we're about to snap to.	Also, we'll call
		// this several times per frame, so we really need to avoid sticking to the bottom of
		// water on each call, and the converse case will correct itself if called twice.
		//CheckWater();

		var point = Position - Vector3.Up * 2;
		var vBumpOrigin = Position;

		//
		//  Shooting up really fast.  Definitely not on ground trimed until ladder shit
		//
		var bMovingUpRapidly = Velocity.z + _lastBaseVelocity.z > MaxNonJumpVelocity;
		var bMoveToEndPos = false;

		if ( GroundEntity != null ) // and not underwater
		{
			bMoveToEndPos = true;
			point.z -= StepSize;
		}
		else if ( bStayOnGround )
		{
			bMoveToEndPos = true;
			point.z -= StepSize;
		}

		if ( bMovingUpRapidly || Swimming ) // or ladder and moving up
		{
			ClearGroundEntity();
			return;
		}

		var pm = TraceBBox( vBumpOrigin, point, 4.0f );

		if ( pm.Entity == null || Vector3.GetAngle( Vector3.Up, pm.Normal ) > GroundAngle )
		{
			ClearGroundEntity();
			bMoveToEndPos = false;

			if ( Velocity.z + _lastBaseVelocity.z > 0 )
				SurfaceFriction = 0.25f;
		}
		else
		{
			UpdateGroundEntity( pm );
		}

		if ( bMoveToEndPos && !pm.StartedSolid && pm.Fraction > 0.0f && pm.Fraction < 1.0f )
		{
			Position = pm.EndPosition;
		}
	}

	public virtual float GetWishSpeed()
	{
		var ws = Duck.GetWishSpeed();

		if ( ws >= 0 )
			return ws;

		if ( Input.Down( InputButton.Run ) )
			return WalkSpeed;

		return DefaultSpeed;
	}

	public void LimitSpeed()
	{
		var prevz = Velocity.z;
		BaseVelocity = 0;
		Velocity = Velocity.WithZ( 0 ).ClampLength( 290 );
		Velocity = Velocity.WithZ( prevz );
	}

	private void ApplyMomentum()
	{
		if ( !Momentum )
		{
			Velocity += (1.0f + (Time.Delta * 0.5f)) * BaseVelocity;
			BaseVelocity = Vector3.Zero;
		}

		Momentum = false;
	}

	private void BaseSimulate()
	{
		UpdateView();
		CheckLadder();
		Swimming = Pawn.WaterLevel > 0.6f;

		if ( !Swimming /*&& !IsTouchingLadder */)
		{
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
			Velocity += new Vector3( 0, 0, BaseVelocity.z ) * Time.Delta;

			BaseVelocity = BaseVelocity.WithZ( 0 );
		}

		if ( AutoJump ? Input.Down( InputButton.Jump ) : Input.Pressed( InputButton.Jump ) )
		{
			CheckJumpButton();
		}

		var bStartOnGround = GroundEntity != null;
		if ( bStartOnGround )
		{
			Velocity = Velocity.WithZ( 0 );

			if ( GroundEntity != null )
			{
				ApplyFriction( GroundFriction * SurfaceFriction );
			}
		}

		WishVelocity = new Vector3( Input.Forward, Input.Left, 0 );
		var inSpeed = WishVelocity.Length.Clamp( 0, 1 );

		if ( !Swimming )
		{
			WishVelocity *= Input.Rotation.Angles().WithPitch( 0 ).ToRotation();
		}
		else
		{
			WishVelocity *= Input.Rotation.Angles().ToRotation();
		}


		if ( !Swimming/* && !IsTouchingLadder*/ )
		{
			WishVelocity = WishVelocity.WithZ( 0 );
		}

		WishVelocity = WishVelocity.Normal * inSpeed;
		WishVelocity *= GetWishSpeed();

		Duck.PreTick();

		var bStayOnGround = false;
		if ( Swimming )
		{
			if ( Pawn.WaterLevel.AlmostEqual( 0.6f, .05f ) )
				CheckWaterJump();

			WaterMove();
		}
		//else if ( IsTouchingLadder )
		//{
		//	LadderMove();
		//}
		else if ( GroundEntity != null )
		{
			bStayOnGround = true;
			WalkMove();
		}
		else
		{
			AirMove();
		}

		CategorizePosition( bStayOnGround );

		// FinishGravity
		if ( !Swimming/* && !IsTouchingLadder*/ )
		{
			Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;
		}

		if ( GroundEntity != null )
		{
			Velocity = Velocity.WithZ( 0 );
		}
	}

	public virtual void CheckJumpButton()
	{
		if ( Swimming )
		{
			ClearGroundEntity();

			Velocity = Velocity.WithZ( 100 );
			return;
		}

		if ( GroundEntity == null )
			return;

		ClearGroundEntity();

		Velocity = Velocity.WithZ( JumpSpeed );
		// Velocity -= new Vector3( 0, 0, Gravity * 0.5f ) * Time.Delta;

		AddEvent( "jump" );
	}

	[Net, Predicted]
	public TimeSince TimeSinceWaterJump { get; set; }
	private void CheckWaterJump()
	{
		if ( !Input.Down( InputButton.Jump ) )
			return;

		if ( TimeSinceWaterJump < 2f )
			return;

		if ( Velocity.z < -180 )
			return;

		var fwd = Rotation * Vector3.Forward;
		var flatvelocity = Velocity.WithZ( 0 );
		var curspeed = flatvelocity.Length;
		flatvelocity = flatvelocity.Normal;
		var flatforward = fwd.WithZ( 0 ).Normal;

		// Are we backing into water from steps or something?  If so, don't pop forward
		if ( !curspeed.AlmostEqual( 0f ) && (Vector3.Dot( flatvelocity, flatforward ) < 0f) )
			return;

		var vecstart = Position + (mins + maxs) * .5f;
		var vecend = vecstart + flatforward * 24f;

		var tr = TraceBBox( vecstart, vecend, 0f );
		if ( tr.Fraction < 1.0f )
		{
			const float WATERJUMP_HEIGHT = 8f;
			vecstart.z += Position.z + EyeHeight + WATERJUMP_HEIGHT;

			vecend = vecstart + flatforward * 24f;

			tr = TraceBBox( vecstart, vecend );
			if ( tr.Fraction == 1.0f )
			{
				vecstart = vecend;
				vecend.z -= 1024f;
				tr = TraceBBox( vecstart, vecend );
				if ( (tr.Fraction < 1.0f) && (tr.Normal.z >= 0.7f) )
				{
					Velocity = Velocity.WithZ( 356f );
					TimeSinceWaterJump = 0f;
				}
			}
		}
	}

	public virtual void WaterMove()
	{
		var wishvel = WishVelocity;

		if ( Input.Down( InputButton.Jump ) )
		{
			wishvel[2] += DefaultSpeed;
		}
		else if ( wishvel.IsNearlyZero() )
		{
			wishvel[2] -= 40f;
		}
		else
		{
			var upwardMovememnt = Input.Forward * (Rotation * Vector3.Forward).z * 2;
			upwardMovememnt = Math.Clamp( upwardMovememnt, 0f, DefaultSpeed );
			wishvel[2] += Input.Up + upwardMovememnt;
		}

		var speed = Velocity.Length;
		var wishspeed = Math.Min( wishvel.Length, DefaultSpeed ); // * 0.8f;
		var wishdir = wishvel.Normal;

		if ( speed > 0 )
		{
			var newspeed = speed - Time.Delta * speed * GroundFriction * SurfaceFriction;
			if ( newspeed < 0.1f )
			{
				newspeed = 0;
			}

			Velocity *= newspeed / speed;
		}

		if ( wishspeed >= 0.1f )  // old !
		{
			Accelerate( wishdir, wishspeed, 100, Acceleration );
		}

		Velocity += BaseVelocity;

		Move();

		Velocity -= BaseVelocity;
	}

	/// <summary>
	/// Calculate the orientation of the view from the hull.
	/// </summary>
	public void UpdateView( bool snapCamera = false )
	{
		// var eyeZ = Duck.Ducked ? DuckHeight - (BodyHeight - EyeHeight) : EyeHeight;
		var eyeZ = Duck.EyeHeight();
		EyeLocalPosition = Vector3.Up * (eyeZ * Pawn.Scale);

		UpdateBBox();
		EyeLocalPosition += TraceOffset;
		EyeRotation = Input.Rotation;

		if ( snapCamera )
			this.Pawn.ResetInterpolation();
	}

}

