using Sandbox;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TTT;

public abstract partial class Grenade : Carriable
{
	private enum ThrowType
	{
		None,
		Overhand,
		Underhand
	}

	[Net, Predicted]
	private TimeUntil TimeUntilExplode { get; set; }

	public override List<UI.BindingTip> BindingTips => new()
	{
		new( InputButton.PrimaryAttack, "Throw" ),
		new( InputButton.SecondaryAttack, "Underhand" ),
	};

	protected virtual float SecondsUntilExplode => 3f;
	private ThrowType _throw = ThrowType.None;
	private bool _isThrown = false;

	public override bool CanCarry( Player carrier )
	{
		return !_isThrown && base.CanCarry( carrier );
	}

	public override void Simulate( IClient client )
	{
		if ( _throw == ThrowType.None )
		{
			if ( Input.Pressed( InputButton.PrimaryAttack ) )
				_throw = ThrowType.Overhand;
			else if ( Input.Pressed( InputButton.SecondaryAttack ) )
				_throw = ThrowType.Underhand;

			if ( _throw != ThrowType.None )
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

			var forwards = PreviousOwner.EyeRotation.Forward;
			forwards *= _throw == ThrowType.Overhand ? 800f : 300f;

			var upwards = PreviousOwner.EyeRotation.Up * 200f;

			Velocity = PreviousOwner.Velocity + forwards + upwards;
			Position = PreviousOwner.EyePosition + PreviousOwner.EyeRotation.Forward * 3.0f + Vector3.Down * 10f;

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
