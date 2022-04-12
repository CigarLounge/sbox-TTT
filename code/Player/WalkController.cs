using Sandbox;
using System;

namespace TTT;

public class Duck : Sandbox.Duck
{
	public Duck( BasePlayerController controller ) : base( controller ) { }

	public override float GetWishSpeed()
	{
		return IsActive ? 90.0f : -1;
	}
}

public partial class WalkController : Sandbox.WalkController
{
	public WalkController() : base()
	{
		Duck = new Duck( this );
		DefaultSpeed = 237.0f;
		WalkSpeed = 110.0f;
		StopSpeed = 150.0f;
	}

	public float FallVelocity { get; private set; } = 0;
	public float FallPunchThreshold = 350f;
	public float DamageForFallSpeed => 100.0f / (FatalFallSpeed - SafeFallSpeed);
	public float SafeFallSpeed => MathF.Sqrt( 2 * Gravity * 20 * 14 );
	public float FatalFallSpeed => MathF.Sqrt( 2 * Gravity * 60 * 12 );

	public override void Simulate()
	{
		FallVelocity = -Pawn.Velocity.z;

		base.Simulate();

		if ( GroundEntity is null || FallVelocity <= 0 )
			return;

		if ( Pawn.IsAlive() && FallVelocity >= FallPunchThreshold && Pawn.WaterLevel == 0f )
		{
			_ = new Sandbox.ScreenShake.Perlin( 1, 1, FallVelocity.LerpInverse( FallPunchThreshold, FallPunchThreshold * 3 ), 1 );

			if ( GroundEntity.Velocity.z < 0.0f )
			{
				FallVelocity += GroundEntity.Velocity.z;
				FallVelocity = MathF.Max( 0.1f, FallVelocity );
			}

			if ( FallVelocity > SafeFallSpeed )
			{
				Pawn.TakeDamage( new DamageInfo
				{
					Flags = DamageFlags.Fall,
					Damage = (FallVelocity - SafeFallSpeed) * DamageForFallSpeed,
					Attacker = Pawn,
				} );
			}
		}
	}

	public override float GetWishSpeed()
	{
		float ws = Duck.GetWishSpeed();

		if ( ws >= 0 )
			return ws;

		if ( Input.Down( InputButton.Run ) )
			return WalkSpeed;

		return DefaultSpeed;
	}
}
