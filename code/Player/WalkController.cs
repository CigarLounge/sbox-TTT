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

	public const float FallDamageThreshold = 650f;
	public const float FallDamageScale = 0.3f;

	public override void Simulate()
	{

		base.Simulate();

		var fallVelocity = -Pawn.Velocity.z;
		if ( GroundEntity is null || fallVelocity <= 0 )
			return;

		if ( fallVelocity > FallDamageThreshold )
		{
			_ = new Sandbox.ScreenShake.Perlin( 1f, 0.2f, 2f );

			var damage = (MathF.Abs( fallVelocity ) - FallDamageThreshold) * FallDamageScale;
			Pawn.TakeDamage( new DamageInfo
			{
				Attacker = Pawn,
				Flags = DamageFlags.Fall,
				Force = Vector3.Down * Velocity.Length,
				Damage = damage,
			} );
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
