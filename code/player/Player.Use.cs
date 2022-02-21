using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Predicted]
	public new Entity Using { get; set; }

	public bool IsUseDisabled()
	{
		return ActiveChild is IUse use && use.IsUsable( this );
	}

	protected override void TickPlayerUse()
	{
		if ( Input.Pressed( InputButton.Use ) )
		{
			Using = FindUsable();

			if ( Using == null )
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

	protected override void StopUsing()
	{
		Using = null;
	}

	protected override Entity FindUsable()
	{
		if ( IsUseDisabled() )
			return null;

		// First try a direct 0 width line
		var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (105 * Scale) )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

		// See if any of the parent entities are usable if we ain't.
		var ent = tr.Entity;
		while ( ent.IsValid() && !IsValidUseEntity( ent ) )
		{
			ent = ent.Parent;
		}

		// Nothing found, try a wider search
		if ( !IsValidUseEntity( ent ) )
		{
			tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * (105 * Scale) )
			.Radius( 2 )
			.HitLayer( CollisionLayer.Debris )
			.Ignore( this )
			.Run();

			// See if any of the parent entities are usable if we ain't.
			ent = tr.Entity;
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
