using Sandbox;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 5000f;

	private static UI.EntityHintPanel s_currentHintPanel;
	private static IEntityHint s_currentHint;

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

		if ( hint == s_currentHint )
		{
			hint.Tick( CurrentPlayer );
			return;
		}

		DeleteHint();

		s_currentHintPanel = hint.DisplayHint( CurrentPlayer );
		s_currentHintPanel.Parent = UI.HintDisplay.Instance;
		s_currentHintPanel.Enabled( true );

		s_currentHint = hint;
	}

	private static void DeleteHint()
	{
		s_currentHintPanel?.Delete( true );
		s_currentHintPanel = null;
		UI.FullScreenHintMenu.Instance?.Close();

		s_currentHint = null;
	}

	private IEntityHint FindHintableEntity()
	{
		var trace = Trace.Ray( CurrentView.Position, CurrentView.Position + CurrentView.Rotation.Forward * MaxHintDistance )
			.Ignore( CurrentPlayer )
			.WithAnyTags( "solid", "trigger" )
			.Run();

		HoveredEntity = trace.Entity;

		if ( HoveredEntity is IEntityHint hint && trace.Distance <= hint.HintDistance )
			return hint;

		return null;
	}
}
