using Sandbox;
using Sandbox.Component;
using Sandbox.UI;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 5000f;

	private static Panel _currentHintPanel;
	private static IEntityHint _currentHint;

	private void DisplayEntityHints()
	{
		var tr = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * MaxHintDistance )
			.Ignore( UI.Hud.DisplayedPlayer )
			.WithAnyTags( "solid", "interactable" )
			.UseHitboxes()
			.Run();

		HoveredEntity = tr.Entity;

		if ( HoveredEntity is not IEntityHint hint || tr.Distance > hint.HintDistance || hint is null || !hint.CanHint( UI.Hud.DisplayedPlayer ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( UI.Hud.DisplayedPlayer );
			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( UI.Hud.DisplayedPlayer );
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
}
