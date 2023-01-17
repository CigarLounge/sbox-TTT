using Sandbox;
using Sandbox.Physics;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	private ModelEntity GrabbedEntity { get; set; }
	public string PrimaryAttackHint => GrabbedEntity.IsValid() ? "Throw" : string.Empty;
	public string SecondaryAttackHint => GrabbedEntity.IsValid() ? "Drop" : string.Empty;
	public bool IsHolding => GrabbedEntity.IsValid() || _isThrowing;

	private readonly Player _owner;
	private bool _isThrowing = false;
	private readonly bool _isInteractable = false;
	private PhysicsJoint _weld;

	public GrabbableProp( Player owner, ModelEntity grabPoint, ModelEntity grabbedEntity, int bone )
	{
		_owner = owner;

		// We want to be able to shoot whatever entity the player is holding.
		// Let's give it a tag that interacts with bullets and doesn't collide with players.
		_isInteractable = grabbedEntity.Tags.Has( "interactable" );
		if ( !_isInteractable )
			grabbedEntity.Tags.Add( "interactable" );

		GrabbedEntity = grabbedEntity;
		GrabbedEntity.EnableTouch = false;
		GrabbedEntity.EnableHideInFirstPerson = false;
		_weld = grabPoint.GmodWeld( grabbedEntity, bone );
	}

	public void Update( Player player )
	{
		// Incase someone walks up and picks up the carriable from the player's hands
		// we just need to reset "EnableHideInFirstPerson".
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
			if ( !_isInteractable )
			{
				grabbedEntity.Tags.Remove( "interactable" );
				grabbedEntity.Components.GetOrCreate<IgnoreDamage>();
			}

			grabbedEntity.LastAttacker = _owner;
			grabbedEntity.EnableHideInFirstPerson = true;
			grabbedEntity.EnableTouch = true;
			_weld.Remove();
			_weld = null;

			if ( grabbedEntity is Carriable carriable )
				carriable.OnCarryDrop( _owner );
		}

		GrabbedEntity = null;
		return grabbedEntity;
	}

	public void SecondaryAction()
	{
		_isThrowing = true;

		var droppedEntity = Drop();
		if ( droppedEntity.IsValid() )
			droppedEntity.Velocity = _owner.GetDropVelocity();

		_owner.SetAnimParameter( "b_attack", true );
		Utils.DelayAction( 0.6f, () => _isThrowing = false );
	}
}
