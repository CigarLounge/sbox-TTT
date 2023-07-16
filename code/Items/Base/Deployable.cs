using Sandbox;
using System;
using System.Collections.Generic;

namespace TTT;

public abstract class Deployable<T> : Carriable where T : ModelEntity, new()
{
	public GhostEntity GhostEntity { get; private set; }

	public override List<UI.BindingPrompt> BindingPrompts => new()
	{
		new( InputAction.PrimaryAttack, CanDrop ? "Deploy" : string.Empty ),
		new( InputAction.SecondaryAttack, CanPlant ? "Plant" : string.Empty ),
	};

	protected virtual bool CanDrop => true;
	protected virtual bool CanPlant => true;

	public override void ActiveStart( Player player )
	{
		base.ActiveStart( player );

		EnableDrawing = false;

		if ( !CanPlant )
			return;

		GhostEntity = new();
		GhostEntity.SetEntity( this );
	}

	public override void ActiveEnd( Player player, bool dropped )
	{
		base.ActiveEnd( player, dropped );

		GhostEntity?.Delete();
	}

	public override void Simulate( IClient client )
	{
		if ( !Game.IsServer )
			return;

		if ( CanDrop && Input.Pressed( InputAction.PrimaryAttack ) )
		{
			OnDeploy( Owner.Inventory.DropEntity( this ) );
			return;
		}

		if ( !CanPlant || !Input.Pressed( InputAction.SecondaryAttack ) )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.StaticOnly()
			.Run();

		if ( !trace.Hit )
			return;

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

		var valid = GhostEntity.IsPlacementValid( ref trace );

		if ( !valid )
			return;

		var dropped = Owner.Inventory.DropEntity( this );
		dropped.PhysicsEnabled = false;
		dropped.Transform = GhostEntity.Transform;
		dropped.Velocity = 0;

		OnDeploy( dropped );
	}

	public override void FrameSimulate( IClient client )
	{
		base.FrameSimulate( client );

		if ( !GhostEntity.IsValid() )
			return;

		var trace = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * Player.UseDistance )
			.StaticOnly()
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

		var valid = GhostEntity.IsPlacementValid( ref trace );

		if ( valid )
			GhostEntity.ShowValid();
		else
			GhostEntity.ShowInvalid();
	}

	protected virtual void OnDeploy( T entity ) { }
}
