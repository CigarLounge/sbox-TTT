using Sandbox;

namespace TTT;

[ClassName( "ttt_equipment_mantis" )]
[Title( "Mantis-Manipulator" )]
public partial class MantisManipulator : Carriable
{
	public class PickedUp : EntityComponent<Entity> { }

	private Entity GrabbedEntity { get; set; }

	public override void Simulate( IClient client )
	{
		if ( !Game.IsServer )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.UseHitboxes()
			.Ignore( Owner )
			.WithAnyTags( "solid", "interactable" )
			.EntitiesOnly()
			.Run();

		if ( GrabbedEntity.IsValid() )
		{
			if ( ShouldDrop( trace ) )
			{
				GrabbedEntity?.Components.RemoveAny<PickedUp>();
				GrabbedEntity = null;
			}
			else
			{
				var velocity = GrabbedEntity.Velocity;
				Vector3.SmoothDamp( GrabbedEntity.Position, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance, ref velocity, 0.2f, Time.Delta );
				GrabbedEntity.AngularVelocity = Angles.Zero;
				GrabbedEntity.Velocity = velocity.ClampLength( 300f );
			}

			return;
		}

		if ( CanPickup( trace.Entity ) )
		{
			GrabbedEntity = trace.Entity;
			GrabbedEntity.Components.GetOrCreate<PickedUp>();
		}
	}

	private bool CanPickup( Entity ent )
	{
		// TODO:
		// Check if anyone is standing ontop of it.
		// Check for size and width of the model.
		return Input.Pressed( InputButton.SecondaryAttack )
			&& ent is ModelEntity model
			&& model.PhysicsEnabled
			&& model.Components.Get<PickedUp>() is null;
	}

	private bool ShouldDrop( TraceResult tr )
	{
		return Input.Pressed( InputButton.SecondaryAttack )
		|| Input.Pressed( InputButton.PrimaryAttack )
		|| tr.EndPosition.Distance( GrabbedEntity.Position ) > 70f
		|| GrabbedEntity.Components.Get<PickedUp>() is null;
	}
}
