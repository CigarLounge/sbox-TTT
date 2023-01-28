using Sandbox;
using Sandbox.Component;

namespace TTT;

public partial class Player
{
	[Net]
	public Entity Using { get; protected set; }

	/// <summary>
	/// The entity we're currently looking at.
	/// </summary>
	public Entity HoveredEntity { get; private set; }

	public const float UseDistance = 80f;
	private float _traceDistance;

	public bool CanUse( Entity entity )
	{
		if ( entity is not IUse use )
			return false;

		if ( !use.IsUsable( this ) )
			return false;

		if ( _traceDistance > UseDistance && FindUsablePoint( entity ) is null )
			return false;

		return true;
	}

	public bool CanContinueUsing( Entity entity )
	{
		if ( HoveredEntity != entity )
			return false;

		if ( _traceDistance > UseDistance && FindUsablePoint( entity ) is null )
			return false;

		if ( entity is IUse use && use.OnUse( this ) )
			return true;

		return false;
	}

	public void StartUsing( Entity entity )
	{
		Using = entity;
	}

	protected void PlayerUse()
	{
		var lastHoveredEntity = HoveredEntity;
		HoveredEntity = FindHovered();

		if ( Game.IsClient )
		{
			if ( HoveredEntity == null || HoveredEntity != lastHoveredEntity )
				SetGlow( lastHoveredEntity, false );
			else
				SetGlow( HoveredEntity, true );
		}

		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Use ) )
			{
				if ( CanUse( HoveredEntity ) )
					StartUsing( HoveredEntity );
			}

			if ( !Input.Down( InputButton.Use ) )
			{
				StopUsing();
				return;
			}

			// There is no entity to use.
			if ( !Using.IsValid() )
				return;

			if ( !CanContinueUsing( Using ) )
				StopUsing();
		}
	}

	private void SetGlow( Entity ent, bool desiredState )
	{
		if ( Game.IsServer )
			return;

		if ( !ent.IsValid() || ent is not IUse )
			return;

		var canUse = CanUse( ent );
		var glow = ent.Components.GetOrCreate<Glow>();
		glow.Width = 0.25f;
		glow.Color = canUse ? Role.Color : Color.Gray;
		glow.Enabled = desiredState && canUse;
	}

	protected Entity FindHovered()
	{
		var trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * MaxHintDistance )
			.Ignore( this )
			.WithAnyTags( "solid", "interactable" )
			.Run();

		if ( !trace.Entity.IsValid() )
			return null;

		if ( trace.Entity.IsWorld )
			return null;

		_traceDistance = trace.Distance;

		return trace.Entity;
	}

	protected void StopUsing()
	{
		Using = null;
	}

	private Vector3? FindUsablePoint( Entity entity )
	{
		if ( entity is null || entity.PhysicsGroup is null || entity.PhysicsGroup.BodyCount == 0 )
			return null;

		foreach ( var body in entity.PhysicsGroup.Bodies )
		{
			var usablePoint = body.FindClosestPoint( EyePosition );

			if ( EyePosition.Distance( usablePoint ) <= UseDistance )
				return usablePoint;
		}

		return null;
	}
}
