using Sandbox;

namespace TTT;

public partial class Player
{
	private UI.EntityHintPanel _currentHintPanel;
	private IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		if ( !IsFirstPersonMode )
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

	public IEntityHint IsLookingAtHintableEntity( float maxHintDistance )
	{
		var trace = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * maxHintDistance );
		trace = trace.HitLayer( CollisionLayer.Debris ).Ignore( CurrentPlayer );

		TraceResult tr = trace.UseHitboxes().Run();

		if ( tr.Hit && tr.Entity is IEntityHint hint && tr.StartPosition.Distance( tr.EndPosition ) <= hint.HintDistance )
			return hint;

		return null;
	}
}
