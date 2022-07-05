using Sandbox;

namespace TTT;

public partial class Player
{
	public Entity Using { get; protected set; }

	public const float UseDistance = 80f;

	public bool IsUseDisabled()
	{
		return ActiveChild is IUse use && use.IsUsable( this );
	}

	protected void PlayerUse()
	{
		// Turn prediction off
		using ( Prediction.Off() )
		{
			if ( Input.Pressed( InputButton.Use ) )
			{
				Using = FindUsable();

				if ( Using is null )
				{
					UseFail();
					return;
				}
			}

			if ( !Input.Down( InputButton.Use ) )
			{
				StopUsing();
				return;
			}

			if ( !Using.IsValid() )
				return;

			// If we move too far away or something we should probably ClearUse()?

			//
			// If use returns true then we can keep using it
			//
			if ( Using is IUse use && use.OnUse( this ) )
				return;

			StopUsing();
		}
	}

	protected Entity FindUsable()
	{
		if ( IsUseDisabled() )
			return null;

		// First try a direct 0 width line.
		var trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (UseDistance * Scale) )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

		// See if any of the parent entities are usable if we ain't.
		var entity = trace.Entity;
		while ( entity.IsValid() && !IsValidUseEntity( entity ) )
		{
			entity = entity.Parent;
		}

		// Nothing found, try a wider search.
		if ( !IsValidUseEntity( entity ) )
		{
			trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (UseDistance * Scale) )
			.Radius( 2 )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

			// See if any of the parent entities are usable if we ain't.
			entity = trace.Entity;
			while ( entity.IsValid() && !IsValidUseEntity( entity ) )
			{
				entity = entity.Parent;
			}
		}

		// Still no good? Bail.
		if ( !IsValidUseEntity( entity ) )
			return null;

		return entity;
	}

	/// <summary>
	/// Returns if the entity is a valid usaable entity
	/// </summary>
	protected bool IsValidUseEntity( Entity entity )
	{
		if ( entity is not IUse use )
			return false;

		if ( !use.IsUsable( this ) )
			return false;

		return true;
	}

	/// <summary>
	/// If we're using an entity, stop using it
	/// </summary>
	protected void StopUsing()
	{
		Using = null;
	}

	protected void UseFail()
	{
		if ( IsUseDisabled() )
			return;

		// Do nothing.
	}
}
