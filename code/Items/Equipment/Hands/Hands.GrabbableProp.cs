using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	public string PrimaryAttackHint => _grabbedEntity.IsValid() ? "Throw" : "Pickup";
	public string SecondaryAttackHint => _grabbedEntity.IsValid() ? "Drop" : string.Empty;
	public bool IsHolding => _grabbedEntity is not null || _isThrowing;

	private ModelEntity _grabbedEntity;
	private readonly Player _owner;
	private bool _isThrowing = false;

	public GrabbableProp( Player owner, Entity grabPoint, ModelEntity grabbedEntity )
	{
		_owner = owner;

		_grabbedEntity = grabbedEntity;
		_grabbedEntity.EnableAllCollisions = false;
		_grabbedEntity.EnableTouch = false;
		_grabbedEntity.EnableHideInFirstPerson = false;
		_grabbedEntity.SetParent( grabPoint, Hands.MiddleHandsAttachment, new Transform( Vector3.Zero ) );
	}

	public void Update( Player player )
	{
		// Incase someone walks up and picks up the carriable from the player's hands
		// we just need to reset "EnableHideInFirstPerson", all other parenting is handled on pickup.
		var carriableHasOwner = _grabbedEntity is Carriable && _grabbedEntity.Owner.IsValid();
		if ( carriableHasOwner )
		{
			_grabbedEntity.EnableHideInFirstPerson = true;
			_grabbedEntity = null;
		}

		if ( !_grabbedEntity.IsValid() || !_owner.IsValid() )
			Drop();
	}

	public Entity Drop()
	{
		var grabbedEntity = _grabbedEntity;
		if ( grabbedEntity.IsValid() )
		{
			grabbedEntity.EnableHideInFirstPerson = true;
			grabbedEntity.EnableTouch = true;
			grabbedEntity.EnableAllCollisions = true;
			grabbedEntity.SetParent( null );

			if ( grabbedEntity is Carriable carriable )
				carriable.OnCarryDrop( _owner );
		}

		_grabbedEntity = null;
		return grabbedEntity;
	}

	public void SecondaryAction()
	{
		if ( Host.IsClient )
		{
			_grabbedEntity = null;
			return;
		}

		_isThrowing = true;
		_owner.SetAnimParameter( "b_attack", true );

		var droppedEntity = Drop();
		if ( droppedEntity.IsValid() )
			droppedEntity.Velocity = _owner.Velocity + _owner.EyeRotation.Forward * Player.DropVelocity;

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		_isThrowing = false;
	}
}
