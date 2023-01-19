using Sandbox;
using Sandbox.Physics;

namespace TTT;

// TODO: Give karma penalty for prop killing.
[ClassName( "ttt_equipment_mantis" )]
[Title( "Mantis Manipulator" )]
public partial class MantisManipulator : Carriable
{
	[Net, Local]
	private Entity GrabbedEntity { get; set; }

	public const string PickedUp = "pickedup";

	public override string PrimaryAttackHint => GetPrimaryHint();
	public override string SecondaryAttackHint => GrabbedEntity.IsValid() ? "Drop" : "Pickup";

	private const float MaxPickupMass = 175;
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

			if ( Input.Pressed( InputButton.PrimaryAttack ) )
				TryCorpseAttach();

			return;
		}

		if ( CanPickup( trace.Entity ) )
		{
			GrabbedEntity = trace.Entity;
			GrabbedEntity.Tags.Add( PickedUp );
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

		return !model.Tags.Has( PickedUp );
	}

	private bool ShouldDrop( TraceResult tr )
	{
		return Input.Pressed( InputButton.SecondaryAttack )
		|| tr.EndPosition.Distance( GrabbedEntity.Position ) > 70f
		|| !GrabbedEntity.Tags.Has( PickedUp );
	}

	private void Drop()
	{
		GrabbedEntity.Tags.Remove( PickedUp );
		GrabbedEntity = null;
	}

	private void MoveEntity()
	{
		var velocity = GrabbedEntity.Velocity;
		Vector3.SmoothDamp( GrabbedEntity.Position, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance, ref velocity, 0.2f, Time.Delta );
		GrabbedEntity.AngularVelocity = Angles.Zero;
		GrabbedEntity.Velocity = velocity.ClampLength( GameManager.PsychoMantis ? float.MaxValue : 300f );
	}

	private void TryCorpseAttach()
	{
		if ( GrabbedEntity is not Corpse corpse )
			return;

		if ( corpse.IsAttached )
		{
			corpse.RemoveRopeAttachments();
			return;
		}

		if ( !Owner.Role.CanAttachCorpses )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.Ignore( Owner )
			.Run();

		if ( !trace.Hit || !trace.Entity.IsWorld )
			return;

		var worldLocalPos = trace.Body.Transform.PointToLocal( trace.EndPosition );
		var spring = PhysicsJoint.CreateLength( corpse.PhysicsBody, trace.Body.LocalPoint( worldLocalPos ), 10 );
		spring.SpringLinear = new( 5, 0.3f );
		spring.Collisions = true;

		// We need to turn off prediction in order for the particle to appear on the local client.
		using ( Prediction.Off() )
		{
			var rope = Particles.Create( "particles/rope/rope.vpcf" );
			rope.SetEntityBone( 0, GrabbedEntity, 0 );
			rope.SetPosition( 1, worldLocalPos );
			corpse.AddRopeAttachment( spring, rope );
		}

		Drop();
	}

	private string GetPrimaryHint()
	{
		if ( GrabbedEntity is not Corpse corpse )
			return string.Empty;

		if ( corpse.IsAttached )
			return "Detach";

		return Owner.Role.CanAttachCorpses ? "Attach" : string.Empty;
	}
}
