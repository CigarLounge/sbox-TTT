using Sandbox;

namespace TTT;

partial class Player
{
	// Similar to "IsLookingAtType" but with an extra check ensuring we are within the range
	// of the "HintDistance".
	private IEntityHint IsLookingAtHintableEntity( float maxHintDistance )
	{
		Trace trace;

		if ( IsClient )
		{
			Camera camera = Camera as Camera;

			trace = Trace.Ray( camera.Position, camera.Position + camera.Rotation.Forward * maxHintDistance );
		}
		else
		{
			trace = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * maxHintDistance );
		}

		trace = trace.HitLayer( CollisionLayer.Debris ).Ignore( this );

		if ( IsSpectatingPlayer )
		{
			trace = trace.Ignore( CurrentPlayer );
		}

		TraceResult tr = trace.UseHitboxes().Run();

		if ( tr.Hit && tr.Entity is IEntityHint hint && tr.StartPos.Distance( tr.EndPos ) <= hint.HintDistance )
		{
			return hint;
		}

		return null;
	}
}
