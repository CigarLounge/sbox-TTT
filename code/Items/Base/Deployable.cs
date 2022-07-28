using Sandbox;
using System;

namespace TTT;

public abstract class Deployable<T> : Carriable where T : ModelEntity, new()
{
	public GhostEntity GhostEntity { get; private set; }

	public override string PrimaryAttackHint => CanDrop ? "Deploy" : string.Empty;
	public override string SecondaryAttackHint => CanPlant ? "Plant" : string.Empty;

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

	public override void Simulate( Client client )
	{
		if ( !IsServer )
			return;

		if ( CanDrop && Input.Pressed( InputButton.PrimaryAttack ) )
		{
			OnDeploy( Owner.Inventory.DropEntity( this ) );
			return;
		}

		if ( !CanPlant )
			return;

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

		var valid = GhostEntity.IsPlacementValid( ref trace );

		if ( !valid || !Input.Pressed( InputButton.SecondaryAttack ) )
			return;

		var dropped = Owner.Inventory.DropEntity( this );
		dropped.Transform = GhostEntity.Transform;
		dropped.Velocity = 0;

		OnDeploy( dropped );
	}

	public override void FrameSimulate( Client client )
	{
		base.FrameSimulate( client );

		if ( !GhostEntity.IsValid() )
			return;

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

		var valid = GhostEntity.IsPlacementValid( ref trace );

		if ( valid )
			GhostEntity.ShowValid();
		else
			GhostEntity.ShowInvalid();
	}

	protected virtual void OnDeploy( T entity ) { }
}
