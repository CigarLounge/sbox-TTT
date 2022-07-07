using Sandbox;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 5000f;

	private static UI.EntityHintPanel _currentHintPanel;
	private static IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		if ( !CurrentPlayer.IsFirstPersonMode )
		{
			DeleteHint();
			return;
		}

		var hint = FindHintableEntity();
		if ( hint is null || !hint.CanHint( CurrentPlayer ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( CurrentPlayer );
			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( CurrentPlayer );
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

	private IEntityHint FindHintableEntity()
	{
		var trace = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * MaxHintDistance )
			.Ignore( CurrentPlayer )
			.HitLayer( CollisionLayer.Debris )
			.HitLayer( CollisionLayer.Solid )
			.UseHitboxes()
			.Run();

		HoveredEntity = trace.Entity;

		if ( HoveredEntity is IEntityHint hint && trace.StartPosition.Distance( trace.EndPosition ) <= hint.HintDistance )
			return hint;

		return null;
	}
}
