using Sandbox;

namespace TTT;

public partial class Player
{
	public const float UseDistance = 80f;

	public bool IsUseDisabled()
	{
		return ActiveChild is IUse use && use.IsUsable( this );
	}

	protected void PlayerUse()
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

		if ( Using is IUse use && use.OnUse( this ) )
			return;

		StopUsing();
	}

	protected override Entity FindUsable()
	{
		if ( IsUseDisabled() )
			return null;

		// First try a direct 0 width line.
		var trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (UseDistance * Scale) )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

		// See if any of the parent entities are usable if we ain't.
		var ent = trace.Entity;
		while ( ent.IsValid() && !IsValidUseEntity( ent ) )
		{
			ent = ent.Parent;
		}

		// Nothing found, try a wider search.
		if ( !IsValidUseEntity( ent ) )
		{
			trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (UseDistance * Scale) )
			.Radius( 2 )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

			// See if any of the parent entities are usable if we ain't.
			ent = trace.Entity;
			while ( ent.IsValid() && !IsValidUseEntity( ent ) )
			{
				ent = ent.Parent;
			}
		}

		// Still no good? Bail.
		if ( !IsValidUseEntity( ent ) )
			return null;

		return ent;
	}

	protected override void UseFail()
	{
		if ( IsUseDisabled() )
			return;

		// do nothing
	}
}
