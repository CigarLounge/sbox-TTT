using System;

using Sandbox;

namespace TTT;

public partial class DefaultWalkController : WalkController
{
	public const float FALL_DAMAGE_VELOCITY = 630f;
	public const float FALL_DAMAGE_SCALE = 0.20f;

	public const float MAX_BREATH = 100f;
	public const float BREATH_LOSS_PER_SECOND = 10f;
	public const float BREATH_GAIN_PER_SECOND = 50f;
	public const float DROWN_DAMAGE_PER_SECOND = 20f;

	[Net, Local]
	public float Breath { get; set; } = 100f;

	public bool IsUnderwater { get; set; } = false;

	private float _fallVelocity;

	public DefaultWalkController() : base()
	{
		Duck = new Duck( this );
		DefaultSpeed = 237f;
		WalkSpeed = 110f;
		GroundFriction = 7.0f;
	}

	public override void Simulate()
	{
		#region Drowning
		IsUnderwater = Pawn.WaterLevel.Fraction == 1f;

		if ( IsUnderwater )
		{
			Breath = MathF.Max( Breath - BREATH_LOSS_PER_SECOND * Time.Delta, 0f );
		}
		else
		{
			Breath = MathF.Min( Breath + BREATH_GAIN_PER_SECOND * Time.Delta, MAX_BREATH );
		}

		if ( Host.IsServer && Breath == 0f )
		{
			using ( Prediction.Off() )
			{
				DamageInfo damageInfo = new()
				{
					Attacker = Pawn,
					Flags = DamageFlags.Drown,
					HitboxIndex = (int)HitboxIndex.Head,
					Position = Position,
					Damage = MathF.Max( DROWN_DAMAGE_PER_SECOND * Time.Delta, 0f )
				};

				Pawn.TakeDamage( damageInfo );
			}
		}
		#endregion

		OnPreTickMove();

		base.Simulate();
	}

	public void OnPreTickMove()
	{
		_fallVelocity = Velocity.z;
	}

	public override float GetWishSpeed()
	{
		var ws = Duck.GetWishSpeed();
		if ( ws >= 0 ) return ws;

		if ( Input.Down( InputButton.Run ) ) return WalkSpeed;

		return DefaultSpeed;
	}

	public override void CategorizePosition( bool stayOnGround )
	{
		base.CategorizePosition( stayOnGround );

		Vector3 point = Position - Vector3.Up * 2;

		if ( GroundEntity != null || stayOnGround )
		{
			point.z -= StepSize;
		}

		TraceResult pm = TraceBBox( Position, point, 4.0f );

		OnPostCategorizePosition( stayOnGround, pm );
	}

	public virtual void OnPostCategorizePosition( bool stayOnGround, TraceResult trace )
	{
		if ( Host.IsServer && trace.Hit && _fallVelocity < -FALL_DAMAGE_VELOCITY )
		{
			using ( Prediction.Off() )
			{
				var totalDamage = Math.Floor( (MathF.Abs( _fallVelocity ) - FALL_DAMAGE_VELOCITY) * FALL_DAMAGE_SCALE );
				if ( totalDamage <= 0 )
				{
					return;
				}

				DamageInfo damageInfo = new()
				{
					Attacker = Pawn,
					Flags = DamageFlags.Fall,
					HitboxIndex = (int)HitboxIndex.LeftFoot,
					Position = Position,
					Damage = (float)totalDamage
				};

				Pawn.TakeDamage( damageInfo );
			}
		}
	}
}
