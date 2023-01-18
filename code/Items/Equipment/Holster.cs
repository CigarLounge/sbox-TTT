using Sandbox;

namespace TTT;

[ClassName( "ttt_equipment_holster" )]
[Title( "Holster" )]
public partial class Holster : Carriable
{
	private ModelEntity GrabbedEntity { get; set; }

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

		DebugOverlay.TraceResult( trace );

		if ( GrabbedEntity.IsValid() )
		{
			if ( Input.Pressed( InputButton.Use ) || trace.EndPosition.Distance( GrabbedEntity.Position ) > 75f )
			{
				GrabbedEntity = null;
				return;
			}

			var velocity = GrabbedEntity.Velocity;
			Vector3.SmoothDamp( GrabbedEntity.Position, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance / 2, ref velocity, 0.2f, Time.Delta * 2f );
			GrabbedEntity.AngularVelocity = Angles.Zero;
			GrabbedEntity.Velocity = velocity.ClampLength( 400f );
		}

		if ( !Input.Pressed( InputButton.Use ) )
			return;

		if ( !trace.Hit || trace.Entity is not ModelEntity model || !model.PhysicsEnabled )
			return;

		GrabbedEntity = model;
	}
}
