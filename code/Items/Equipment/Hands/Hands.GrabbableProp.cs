using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	public ModelEntity GrabbedEntity { get; set; }

	private readonly Player _owner;
	public bool IsHolding => GrabbedEntity is not null || _isThrowing;
	private bool _isThrowing = false;

	public GrabbableProp( Player owner, Entity grabPoint, ModelEntity grabbedEntity )
	{
		_owner = owner;

		GrabbedEntity = grabbedEntity;
		GrabbedEntity.EnableAllCollisions = false;
		GrabbedEntity.EnableTouch = false;
		GrabbedEntity.EnableHideInFirstPerson = false;
		GrabbedEntity.SetParent( grabPoint, Hands.MiddleHandsAttachment, new Transform( Vector3.Zero ) );
	}

	public void Update( Player player )
	{
		// Incase someone walks up and picks up the carriable from the player's hands
		// we just need to reset "EnableHideInFirstPerson", all other parenting is handled on pickup.
		var carriableHasOwner = GrabbedEntity is Carriable && GrabbedEntity.Owner.IsValid();
		if ( carriableHasOwner )
		{
			GrabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity = null;
		}

		if ( !GrabbedEntity.IsValid() || !_owner.IsValid() )
			Drop();
	}

	public Entity Drop()
	{
		var grabbedEntity = GrabbedEntity;
		if ( grabbedEntity.IsValid() )
		{
			grabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity.EnableTouch = true;
			grabbedEntity.EnableAllCollisions = true;
			grabbedEntity.SetParent( null );

			if ( grabbedEntity is Carriable carriable )
				carriable.OnCarryDrop( _owner );
		}

		GrabbedEntity = null;
		return grabbedEntity;
	}

	public virtual void SecondaryAction()
	{
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
