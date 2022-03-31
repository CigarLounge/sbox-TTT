using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public abstract partial class Throwable<T> : Carriable where T : BaseGrenade, new()
{
	public override void Simulate( Client client )
	{
		if ( TimeSinceDropped < Info.DeployTime )
			return;

		if ( Input.Released( InputButton.Attack1 ) )
			Throw();
	}

	private void Throw()
	{
		Rand.SetSeed( Time.Tick );

		Owner.SetAnimParameter( "b_attack", true );

		if ( !IsServer )
			return;

		using ( Prediction.Off() )
		{
			var grenade = new T
			{
				Position = Owner.EyePosition + Owner.EyeRotation.Forward * 3.0f,
				Owner = Owner
			};

			grenade.PhysicsBody.Velocity = Owner.EyeRotation.Forward * 600.0f + Owner.EyeRotation.Up * 200.0f + Owner.Velocity;

			// This is fucked in the head, lets sort this this year
			grenade.CollisionGroup = CollisionGroup.Debris;
			grenade.SetInteractsExclude( CollisionLayer.Player );
			grenade.SetInteractsAs( CollisionLayer.Debris );

			_ = grenade.BlowIn();
		}

		Delete();
	}
}

public abstract class BaseGrenade : BasePhysics
{
	public static readonly Model WorldModel = Model.Load( "models/weapons/w_frag.vmdl" );
	protected virtual float Seconds => 3f;

	public override void Spawn()
	{
		base.Spawn();

		Model = WorldModel;
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}

	public async Task BlowIn()
	{
		await Task.DelaySeconds( Seconds );

		Explode();
		Delete();
	}

	protected virtual void Explode() { }
}
