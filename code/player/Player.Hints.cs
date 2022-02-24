using Sandbox;

namespace TTT;

public partial class Player
{
	public const float INTERACT_DISTANCE = 80f;
	private const float MAX_HINT_DISTANCE = 20480f;

	private UI.EntityHintPanel _currentHintPanel;
	private IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		if ( Camera is ThirdPersonSpectateCamera )
		{
			DeleteHint();

			return;
		}

		IEntityHint hint = IsLookingAtHintableEntity( MAX_HINT_DISTANCE );

		if ( hint == null || !hint.CanHint( this ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( this );

			_currentHintPanel.UpdateHintPanel( hint.TextOnTick );

			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( this );
		_currentHintPanel.Parent = UI.HintDisplay.Instance;
		_currentHintPanel.Enabled( true );

		_currentHint = hint;
	}

	private void DeleteHint()
	{
		_currentHintPanel?.Delete( true );
		_currentHintPanel = null;
		UI.FullScreenHintMenu.Instance?.Close();

		_currentHint = null;
	}

	// Similar to "IsLookingAtType" but with an extra check ensuring we are within the range
	// of the "HintDistance".
	private IEntityHint IsLookingAtHintableEntity( float maxHintDistance )
	{
		var trace = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * maxHintDistance );
		trace = trace.HitLayer( CollisionLayer.Debris ).Ignore( CurrentPlayer );

		TraceResult tr = trace.UseHitboxes().Run();

		if ( tr.Hit && tr.Entity is IEntityHint hint && tr.StartPosition.Distance( tr.EndPosition ) <= hint.HintDistance )
			return hint;

		return null;
	}
}
