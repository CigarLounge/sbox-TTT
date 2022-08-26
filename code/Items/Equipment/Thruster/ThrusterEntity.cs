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
	private const float BounceForce = 700f;
	private const int MaxBounces = 5;
	private const float TripRadius = 200f;

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
		// TODO: Maybe we shouldn't let people put this on weapons...
		if ( _bounces >= MaxBounces || !Parent.IsValid() || Parent.Parent.IsValid() )
			Delete();

		if ( !_timeUntilActivation )
			return;

		if ( _triggerLight.Color == Color.Red )
		{
			foreach ( var ent in FindInSphere( Position, TripRadius ) )
				if ( ent is Player player && player.IsAlive() )
					_triggerLight.Color = Color.Green;
			return;
		}

		if ( _timeUntilNextBounce )
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

			Particles.Create( Particle, Position );
			Sound.FromWorld( ExplodeSound, Position );
		}

		_bounces += 1;
		_timeUntilNextBounce = 1.5f;
	}
}
