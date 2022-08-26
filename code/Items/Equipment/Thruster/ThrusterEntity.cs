using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_thruster" )]
[Title( "Thruster" )]
[HideInEditor]
public partial class ThrusterEntity : Prop, IEntityHint
{
	private static readonly Model _worldModel = Model.Load( "models/thruster/thruster.vmdl" );

	private const string ExplodeSound = "discombobulator_explode-1";
	private const string Particle = "particles/discombobulator/explode.vpcf";
	private const float BounceForce = 3f;
	private const int MaxBounces = 100;

	private TimeUntil _timeUntilActivation = 0f;
	private int _bounces = 5;
	private TimeUntil _timeUntilNextBounce = 3f;

	public override void Spawn()
	{
		base.Spawn();

		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
		Health = 200f;
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( !_timeUntilActivation )
			return;

		if ( _bounces >= MaxBounces )
			Delete();

		if ( _timeUntilNextBounce )
			Bounce();
	}

	private void Bounce()
	{
		if ( Parent.IsValid() )
		{
			var foundPlayer = false;
			foreach ( var ent in FindInSphere( Parent.Position, 5000 ) )
			{
				if ( ent is Player player && player.IsAlive() )
				{
					foundPlayer = true;
					Parent.Velocity = (player.Position - Parent.Position) * BounceForce;
					break;
				}
			}

			if ( !foundPlayer )
			{
				var randDirection = Rand.Float( -BounceForce, BounceForce );
				Parent.Velocity = new Vector3( randDirection, randDirection, randDirection );
			}

			Particles.Create( Particle, Position );
			Sound.FromWorld( ExplodeSound, Position );
		}

		_bounces += 1;
		_timeUntilNextBounce = 1.5f;
	}
}
