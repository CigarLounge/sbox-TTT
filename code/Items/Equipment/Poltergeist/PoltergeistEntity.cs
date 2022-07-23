using Sandbox;

namespace TTT;

[ClassName( "ttt_entity_poltergeist" )]
[HideInEditor]
public partial class PoltergeistEntity : Prop
{
	private static readonly Model _worldModel = Model.Load( "models/poltergeist/poltergeist_attachment.vmdl" );
	private const int BounceForce = 950;
	private const int MaxBounces = 5;
	private int _bounces = 0;
	private TimeUntil _timeUntilNextBounce = 0f;

	public override void Spawn()
	{
		Model = _worldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	[Event.Tick.Server]
	private void OnServerTick()
	{
		if ( _bounces >= MaxBounces )
			Delete();

		if ( _timeUntilNextBounce )
			Bounce();
	}

	private void Bounce()
	{
		if ( Parent.IsValid() )
		{
			var randDirection = Rand.Float( -BounceForce, BounceForce );
			Parent.Velocity = new Vector3( randDirection, randDirection, randDirection );
		}

		_bounces += 1;
		_timeUntilNextBounce = 1.5f;
	}
}
