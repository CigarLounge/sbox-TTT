using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public abstract partial class Grenade : Carriable
{
	[Net, Predicted]
	public TimeUntil TimeUntilExplode { get; protected set; }

	protected virtual float Seconds => 3f;
	private bool _isThrown = false;

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		TimeUntilExplode = Seconds;
	}

	public override bool CanCarry( Player carrier )
	{
		return !_isThrown && base.CanCarry( carrier );
	}

	public override void Simulate( Client client )
	{
		if ( Input.Pressed( InputButton.PrimaryAttack ) )
			ViewModelEntity?.SetAnimParameter( "fire", true );

		if ( Input.Released( InputButton.PrimaryAttack ) || TimeUntilExplode )
			Throw();

		if ( !Input.Down( InputButton.PrimaryAttack ) )
			TimeUntilExplode = Seconds;
	}

	protected virtual void OnExplode() { }

	protected void Throw()
	{
		if ( !IsServer )
			return;

		using ( Prediction.Off() )
		{
			Owner.Inventory.DropActive();
			Position = PreviousOwner.EyePosition + PreviousOwner.EyeRotation.Forward * 3.0f;
			PhysicsBody.Velocity = PreviousOwner.EyeRotation.Forward * 600.0f + PreviousOwner.EyeRotation.Up * 200.0f + PreviousOwner.Velocity;

			_isThrown = true;
			_ = ExplodeIn( TimeUntilExplode );
		}
	}

	private async Task ExplodeIn( float seconds )
	{
		await GameTask.DelaySeconds( seconds );

		OnExplode();
		Delete();
	}
}
