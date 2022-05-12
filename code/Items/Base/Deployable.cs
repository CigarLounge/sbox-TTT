using Sandbox;
using System;

namespace TTT;

public abstract class Deployable<T> : Carriable where T : ModelEntity, new()
{
	public GhostEntity GhostEntity { get; private set; }

	public override void ActiveStart( Entity entity )
	{
		base.ActiveStart( entity );

		EnableDrawing = false;
		GhostEntity = new();
		GhostEntity.SetEntity( this );
	}

	public override void ActiveEnd( Entity entity, bool dropped )
	{
		base.ActiveEnd( entity, dropped );

		GhostEntity.Delete();
	}

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( !Owner.IsValid() )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.WorldOnly()
			.Run();

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			OnDeploy( Owner.Inventory.DropEntity<T>( this ) );
			return;
		}

		if ( !trace.Hit )
		{
			GhostEntity.EnableDrawing = false;
			return;
		}

		GhostEntity.EnableDrawing = true;
		GhostEntity.Position = trace.EndPosition;
		GhostEntity.Rotation = Rotation.From( trace.Normal.EulerAngles );

		if ( Math.Abs( trace.Normal.z ) >= 0.99f )
		{
			GhostEntity.Rotation = Rotation.From
			(
				Rotation.Angles()
				.WithYaw( Owner.EyeRotation.Yaw() )
				.WithPitch( -90 * trace.Normal.z.CeilToInt() )
				.WithRoll( 180f )
			);
		}

		bool valid = GhostEntity.IsPlacementValid( ref trace );

		if ( !valid || !Input.Pressed( InputButton.Attack2 ) )
			return;

		var dropped = Owner.Inventory.DropEntity<T>( this );
		dropped.MoveType = MoveType.None;
		dropped.Transform = GhostEntity.Transform;
		dropped.Velocity = 0;

		OnDeploy( dropped );
	}

	public override void FrameSimulate( Client client )
	{
		base.FrameSimulate( client );

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.WorldOnly()
			.Run();

		if ( !trace.Hit )
		{
			GhostEntity.EnableDrawing = false;
			return;
		}

		GhostEntity.EnableDrawing = true;
		GhostEntity.Position = trace.EndPosition;
		GhostEntity.Rotation = Rotation.From( trace.Normal.EulerAngles );

		if ( Math.Abs( trace.Normal.z ) >= 0.99f )
		{
			GhostEntity.Rotation = Rotation.From
			(
				Rotation.Angles()
				.WithYaw( Owner.EyeRotation.Yaw() )
				.WithPitch( -90 * trace.Normal.z.CeilToInt() )
				.WithRoll( 180f )
			);
		}

		bool valid = GhostEntity.IsPlacementValid( ref trace );

		if ( valid )
			GhostEntity.ShowValid();
		else
			GhostEntity.ShowInvalid();
	}

	protected virtual void OnDeploy( T entity ) { }
}
