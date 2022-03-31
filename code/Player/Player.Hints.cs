using Sandbox;

namespace TTT;

public partial class Player
{
	public const float MAX_HINT_DISTANCE = 20480f;

	private UI.EntityHintPanel _currentHintPanel;
	private IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		if ( !CurrentPlayer.IsFirstPersonMode )
		{
			DeleteHint();
			return;
		}

		var hint = IsLookingAtHintableEntity( MAX_HINT_DISTANCE );

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
		var trace = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * maxHintDistance )
				.HitLayer( CollisionLayer.Debris )
				.Ignore( CurrentPlayer )
				.UseHitboxes()
				.Run();

		if ( trace.Hit && trace.Entity is IEntityHint hint && trace.StartPosition.Distance( trace.EndPosition ) <= hint.HintDistance )
			return hint;

		return null;
	}
}
