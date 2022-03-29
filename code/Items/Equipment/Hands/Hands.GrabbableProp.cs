using Sandbox;
using System.Threading.Tasks;

namespace TTT;

public class GrabbableProp : IGrabbable
{
	public const float THROW_FORCE = 500;
	public Entity GrabbedEntity { get; set; }
	public Player _owner;

	public bool IsHolding => GrabbedEntity != null;

	public GrabbableProp( Player player, Entity ent )
	{
		_owner = player;

		GrabbedEntity = ent;
		GrabbedEntity.SetParent( player, Hands.MIDDLE_HANDS_ATTACHMENT, new Transform( Vector3.Zero, Rotation.FromRoll( -90 ) ) );
		GrabbedEntity.EnableHideInFirstPerson = false;
	}

	public void Drop()
	{
		if ( GrabbedEntity?.IsValid ?? false )
		{
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

		// If the entity gets another owner (i.e weapon pickup) drop it.
		if ( GrabbedEntity?.Owner != null )
		{
			// Since the weapon now has a new owner/parent, no need to set parent to null.
			GrabbedEntity.EnableHideInFirstPerson = true;
			GrabbedEntity = null;

			return;
		}
	}

	public void SecondaryAction()
	{
		_owner.SetAnimParameter( "b_attack", true );

		GrabbedEntity.SetParent( null );
		GrabbedEntity.EnableHideInFirstPerson = true;
		GrabbedEntity.Velocity += _owner.EyeRotation.Forward * THROW_FORCE;

		_ = WaitForAnimationFinish();
	}

	private async Task WaitForAnimationFinish()
	{
		await GameTask.DelaySeconds( 0.6f );
		GrabbedEntity = null;
	}
}
