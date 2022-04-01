using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public abstract partial class Grenade : Carriable
{
	[Net, Predicted]
	public TimeUntil TimeUntilExplode { get; protected set; }
	protected virtual float Seconds => 3f;

	private bool _isThrown = false;

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		TimeUntilExplode = Seconds;
	}

	public override bool CanCarry( Entity carrier )
	{
		return !_isThrown && base.CanCarry( carrier );
	}

	public override void Simulate( Client client )
	{
		if ( TimeSinceDropped < Info.DeployTime )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
			ViewModelEntity?.SetAnimParameter( "fire", true );

		if ( Input.Released( InputButton.Attack1 ) || TimeUntilExplode )
			Throw();

		if ( !Input.Down( InputButton.Attack1 ) )
			TimeUntilExplode = Seconds;
	}

	public async Task BlowIn( float seconds )
	{
		await Task.DelaySeconds( seconds );

		Explode();
		Delete();
	}

	protected virtual void Explode() { }

	protected void Throw()
	{
		Rand.SetSeed( Time.Tick );

		if ( !IsServer )
			return;

		using ( Prediction.Off() )
		{
			Owner.Inventory.DropActive();
			Position = PreviousOwner.EyePosition + PreviousOwner.EyeRotation.Forward * 3.0f;

			PhysicsBody.Velocity = PreviousOwner.EyeRotation.Forward * 600.0f + PreviousOwner.EyeRotation.Up * 200.0f + PreviousOwner.Velocity;

			// This is fucked in the head, lets sort this this year
			CollisionGroup = CollisionGroup.Debris;
			SetInteractsExclude( CollisionLayer.Player );
			SetInteractsAs( CollisionLayer.Debris );

			_isThrown = true;
			_ = BlowIn( TimeUntilExplode );
		}
	}
}
