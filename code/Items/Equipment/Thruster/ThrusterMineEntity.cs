using System;
using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_thrustermine" )]
[Title( "Thruster Mine" )]
[HideInEditor]
public partial class ThrusterMineEntity : Prop, IEntityHint
{
	private static readonly Model _worldModel = Model.Load( "models/thruster/thruster.vmdl" );

	private const string TriggerSound = "thrustermine-trigger";
	private const string ExplodeSound = "explode";
	private const string ExplodeParticle = "particles/thrustermine/explode.vpcf";
	private const string BounceExplodeSound = "discombobulator_explode-1";
	private const string BounceParticle = "particles/discombobulator/explode.vpcf";
	private const float BounceForce = 700f;
	private const int MaxBounces = 5;
	private const float TripRadius = 200f;

	private bool _isTriggered => _triggerLight.Color == Color.Green;
	private PointLightEntity _triggerLight;
	private int _bounces = 0;
	private TimeUntil _timeUntilActivation = 3f;
	private TimeUntil _timeUntilNextBounce = 3f;

	public override void Spawn()
	{
		Tags.Add( "interaction" );
		Model = _worldModel;
		Health = 200f;

		_triggerLight = new PointLightEntity
		{
			Enabled = true,
			DynamicShadows = true,
			Range = 3,
			Falloff = 1.0f,
			Brightness = 2,
			Color = Color.Red,
			Owner = this,
			Parent = this,
			Rotation = Rotation,
		};
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( _bounces >= MaxBounces || !Parent.IsValid() || Parent.Owner.IsValid() )
		{
			DelayedExplosion( () => { Delete(); } );
			return;
		}

		if ( !_timeUntilActivation )
			return;

		if ( !_isTriggered )
		{
			foreach ( var ent in FindInSphere( Position, TripRadius ) )
			{
				if ( ent is Player player && player.IsAlive() )
				{
					Sound.FromWorld( TriggerSound, Position );
					_triggerLight.Color = Color.Green;
					break;
				}
			}
		}

		if ( _timeUntilNextBounce && _isTriggered )
			Bounce();
	}

	private void Bounce()
	{
		if ( Parent.IsValid() )
		{
			var foundPlayer = false;
			foreach ( var ent in FindInSphere( Parent.Position, TripRadius * 3 ) )
			{
				if ( ent is Player player && player.IsAlive() )
				{
					foundPlayer = true;
					Parent.Velocity = (player.Position - Parent.Position).Normal * BounceForce;
					break;
				}
			}

			if ( !foundPlayer )
			{
				var randDirection = Rand.Float( BounceForce, -BounceForce );
				Parent.Velocity = new Vector3( randDirection, randDirection, randDirection );
			}

			Particles.Create( BounceParticle, Position );
			Sound.FromWorld( BounceExplodeSound, Position );

			DelayedExplosion();
		}

		_bounces += 1;
		_timeUntilNextBounce = 2f;
	}

	private async void DelayedExplosion( Action onFinish = null )
	{
		await GameTask.DelaySeconds( 0.5f );

		if ( !this.IsValid() )
			return;

		Particles.Create( ExplodeParticle, Position );
		Sound.FromWorld( ExplodeSound, Position );

		foreach ( var ent in FindInSphere( Position, TripRadius ) )
		{
			if ( ent is not Player player || !player.IsAlive() )
				continue;

			var dist = Vector3.DistanceBetween( Position, player.PhysicsBody.MassCenter );
			if ( dist > TripRadius )
				continue;

			var distanceMul = 1.0f - Math.Clamp( dist / TripRadius, 0.0f, 1.0f );
			var dmg = 50 * distanceMul;
			var force = distanceMul * player.PhysicsBody.Mass;
			var forceDir = (player.PhysicsBody.MassCenter - Position).Normal;

			var damageInfo = DamageInfo.Explosion( Position, forceDir * force, dmg )
				.WithWeapon( this )
				.WithAttacker( Owner );

			player.TakeDamage( damageInfo );
		}

		onFinish?.Invoke();
	}
}
