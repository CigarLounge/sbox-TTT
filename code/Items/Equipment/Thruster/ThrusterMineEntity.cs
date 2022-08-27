using System;
using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_thrustermine" )]
[Title( "Thruster Mine" )]
[HideInEditor]
public partial class ThrusterMineEntity : Prop, IEntityHint
{
	private static readonly Model _worldModel = Model.Load( "models/thruster/thruster.vmdl" );

	private const float ThrustForce = 700f;
	private const int MaxThrusts = 4;
	private const float TripRadius = 200f;

	private bool _isMineArmed = false;
	private bool _isMineTriggered = false;
	private PointLightEntity _triggerLight;
	private int _thrusts = 0;
	private TimeUntil _timeUntilNextThrust;

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
			Color = Color.Yellow,
			Owner = this,
			Parent = this,
			Rotation = Rotation,
		};

		Arm();
	}

	private async void Arm()
	{
		await GameTask.DelaySeconds( 3f );

		if ( !this.IsValid() )
			return;

		_isMineArmed = true;
		_triggerLight.Color = Color.Red;
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( _thrusts >= MaxThrusts || !Parent.IsValid() || Parent.Owner.IsValid() )
		{
			DelayedExplosion( () => { Delete(); } );
			return;
		}

		if ( !_isMineArmed )
			return;

		if ( !_isMineTriggered )
			CheckTrigger();

		if ( _isMineTriggered && _timeUntilNextThrust )
			Thrust();
	}

	private void CheckTrigger()
	{
		foreach ( var ent in FindInSphere( Position, TripRadius ) )
		{
			if ( ent is Player player && player.IsAlive() )
			{
				Sound.FromWorld( "thrustermine-trigger", Position );
				_triggerLight.Color = Color.Green;
				_timeUntilNextThrust = 1f;
				_isMineTriggered = true;
				break;
			}
		}
	}

	private void Thrust()
	{
		var detectedPlayer = false;
		foreach ( var ent in FindInSphere( Parent.Position, TripRadius * 3 ) )
		{
			if ( ent is Player player && player.IsAlive() )
			{
				detectedPlayer = true;
				Parent.Velocity = (player.Position - Parent.Position).Normal * ThrustForce;
				break;
			}
		}

		// Couldn't find a player within the radius, send it flying in a random direction.
		if ( !detectedPlayer )
		{
			var randDirection = Rand.Float( ThrustForce, -ThrustForce );
			Parent.Velocity = new Vector3( randDirection, randDirection, randDirection );
		}

		Particles.Create( "particles/discombobulator/explode.vpcf", Position );
		Sound.FromWorld( "discombobulator_explode-1", Position );

		DelayedExplosion();

		_thrusts += 1;
		_timeUntilNextThrust = 2f;
	}

	private async void DelayedExplosion( Action onExplode = null )
	{
		await GameTask.DelaySeconds( 0.5f );

		if ( !this.IsValid() )
			return;

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

		Particles.Create( "particles/thrustermine/explode.vpcf", Position );
		Sound.FromWorld( "explode", Position );

		onExplode?.Invoke();
	}
}
