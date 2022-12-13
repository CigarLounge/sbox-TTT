using Sandbox;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 5000f;

	private static UI.EntityHintPanel _currentHintPanel;
	private static IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		if ( !Camera.FirstPersonViewer.IsValid() )
		{
			DeleteHint();
			return;
		}

		var hint = FindHintableEntity();
		if ( hint is null || !hint.CanHint( PlayerCamera.Target ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( PlayerCamera.Target );
			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( PlayerCamera.Target );
		_currentHintPanel.Parent = UI.HintDisplay.Instance;
		_currentHintPanel.Enabled( true );

		_currentHint = hint;
	}

	private static void DeleteHint()
	{
		_currentHintPanel?.Delete( true );
		_currentHintPanel = null;
		UI.FullScreenHintMenu.Instance?.Close();

		_currentHint = null;
	}

	private IEntityHint FindHintableEntity()
	{
		var trace = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * MaxHintDistance )
			.Ignore( PlayerCamera.Target )
			.WithAnyTags( "solid", "interactable" )
			.UseHitboxes()
			.Run();

		HoveredEntity = trace.Entity;

		if ( HoveredEntity is IEntityHint hint && trace.Distance <= hint.HintDistance )
			return hint;

		return null;
	}
}
