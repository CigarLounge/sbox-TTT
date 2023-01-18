using Sandbox;

namespace TTT;

// TODO: Give karma penalty for prop killing.
// TODO: Ability to attach corpses for traitors.
[ClassName( "ttt_equipment_mantis" )]
[Title( "Mantis-Manipulator" )]
public partial class MantisManipulator : Carriable
{
	public class PickedUp : EntityComponent<Entity> { }

	[Net, Local]
	private Entity GrabbedEntity { get; set; }

	public override string SecondaryAttackHint => GrabbedEntity.IsValid() ? "Drop" : "Pickup";

	private const float MaxPickupMass = 205;
	private readonly Vector3 _maxPickupSize = new( 25, 25, 25 );

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
				Drop();
			else
				MoveEntity();

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
		if ( !Input.Pressed( InputButton.SecondaryAttack ) || ent is not ModelEntity model || !model.PhysicsEnabled )
			return false;

		// Bypass all restrictions in the following cases...
		if ( ent is Ammo || ent is Carriable || ent is Corpse )
			return true;

		var size = model.CollisionBounds.Size;
		if ( model.PhysicsGroup.Mass > MaxPickupMass || size.x > _maxPickupSize.x || size.y > _maxPickupSize.y || size.y > _maxPickupSize.z )
			return false;

		// Make sure there is no player standing ontop of it.
		foreach ( var entity in FindInBox( model.CollisionBounds * 2 + model.Position ) )
			if ( entity is Player player && player.GroundEntity == ent )
				return false;

		return model.Components.Get<PickedUp>() is null;
	}

	private bool ShouldDrop( TraceResult tr )
	{
		return Input.Pressed( InputButton.SecondaryAttack )
		|| tr.EndPosition.Distance( GrabbedEntity.Position ) > 70f
		|| GrabbedEntity.Components.Get<PickedUp>() is null;
	}

	private void Drop()
	{
		GrabbedEntity.Components.RemoveAny<PickedUp>();
		GrabbedEntity = null;
	}

	private void MoveEntity()
	{
		var velocity = GrabbedEntity.Velocity;
		Vector3.SmoothDamp( GrabbedEntity.Position, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance, ref velocity, 0.2f, Time.Delta );
		GrabbedEntity.AngularVelocity = Angles.Zero;
		GrabbedEntity.Velocity = velocity.ClampLength( GameManager.PsychoMantis ? float.MaxValue : 300f );
	}
}
