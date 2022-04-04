using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	public const float THROW_FORCE = 500;
	public ModelEntity GrabbedEntity { get; set; }
	public Player _owner;

	public bool IsHolding => GrabbedEntity != null || _isThrowing;
	private bool _isThrowing = false; // Needed to maintain the Holding animation.

	public GrabbableProp( Player player, ModelEntity ent )
	{
		_owner = player;

		GrabbedEntity = ent;
		GrabbedEntity.EnableTouch = false;
		GrabbedEntity.SetParent( player, Hands.MIDDLE_HANDS_ATTACHMENT, new Transform( Vector3.Zero, Rotation.FromRoll( -90 ) ) );
		GrabbedEntity.EnableHideInFirstPerson = false;
	}

	public void Drop()
	{
		if ( GrabbedEntity?.IsValid ?? false )
		{
			GrabbedEntity.EnableTouch = true;
			GrabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity.SetParent( null );
		}

		GrabbedEntity = null;
	}

	public void Update( Player player )
	{
		// If the entity is destroyed drop it.
		if ( !GrabbedEntity?.IsValid ?? true )
		{
			Drop();
			return;
		}
	}

	public void SecondaryAction()
	{
		_isThrowing = true;
		_owner.SetAnimParameter( "b_attack", true );

		GrabbedEntity.Velocity += _owner.EyeRotation.Forward * THROW_FORCE;
		Drop();

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		_isThrowing = false;
	}
}
