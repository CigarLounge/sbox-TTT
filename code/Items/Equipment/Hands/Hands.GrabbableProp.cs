using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	public const float ThrowForce = 500;
	public ModelEntity GrabbedEntity { get; set; }
	public Player _owner;

	public bool IsHolding => GrabbedEntity is not null || _isThrowing;
	private bool _isThrowing = false;

	public GrabbableProp( Player owner, Entity grabPoint, ModelEntity grabbedEntity )
	{
		_owner = owner;

		GrabbedEntity = grabbedEntity;
		GrabbedEntity.EnableTouch = false;
		GrabbedEntity.EnableHideInFirstPerson = false;
		GrabbedEntity.SetParent( grabPoint, Hands.MiddleHandsAttachment, new Transform( Vector3.Zero ) );
	}

	public void Drop()
	{
		if ( GrabbedEntity.IsValid() )
		{
			GrabbedEntity.EnableTouch = true;
			GrabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity.SetParent( null );

			if ( GrabbedEntity is Carriable carriable )
				carriable.OnCarryDrop( _owner );
		}

		GrabbedEntity = null;
	}

	public void Update( Player player )
	{
		if ( !GrabbedEntity.IsValid() || !_owner.IsValid() )
		{
			Drop();
			return;
		}
	}

	public void SecondaryAction()
	{
		_isThrowing = true;
		_owner.SetAnimParameter( "b_attack", true );

		if ( GrabbedEntity.IsValid() )
			GrabbedEntity.Velocity += _owner.EyeRotation.Forward * ThrowForce;
		Drop();

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		_isThrowing = false;
	}
}
