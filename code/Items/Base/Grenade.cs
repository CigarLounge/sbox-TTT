using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public abstract partial class Grenade : Carriable
{
	private enum Action
	{
		None,
		Overhand,
		Underhand
	}

	[Net, Predicted]
	private TimeUntil TimeUntilExplode { get; set; }

	protected virtual float SecondsUntilExplode => 3f;
	private Action _action = Action.None;
	private bool _isThrown = false;

	public override bool CanCarry( Player carrier )
	{
		return !_isThrown && base.CanCarry( carrier );
	}

	public override void Simulate( IClient client )
	{
		if ( _action == Action.None )
		{
			if ( Input.Pressed( InputButton.PrimaryAttack ) )
				_action = Action.Overhand;
			else if ( Input.Pressed( InputButton.SecondaryAttack ) )
				_action = Action.Underhand;

			if ( _action != Action.None )
			{
				ViewModelEntity?.SetAnimParameter( "fire", true );
				TimeUntilExplode = SecondsUntilExplode;
			}

			return;
		}

		if ( TimeUntilExplode || Input.Released( InputButton.PrimaryAttack ) || Input.Released( InputButton.SecondaryAttack ) )
			Throw();
	}

	protected virtual void OnExplode() { }

	protected void Throw()
	{
		if ( !Game.IsServer )
			return;

		using ( Prediction.Off() )
		{
			Owner.Inventory.DropActive();

			var forwards = _action == Action.Overhand ? PreviousOwner.EyeRotation.Forward * 800.0f : PreviousOwner.EyeRotation.Forward * 200f;
			var upwards = _action == Action.Overhand ? PreviousOwner.EyeRotation.Up * 200f : PreviousOwner.EyeRotation.Up * 10f;
			Velocity = PreviousOwner.Velocity + forwards + upwards;
			Position = PreviousOwner.EyePosition + PreviousOwner.EyeRotation.Forward * 3.0f;

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
