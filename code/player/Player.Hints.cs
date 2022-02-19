using Sandbox;

namespace TTT;

public partial class Player
{
	public const float INTERACT_DISTANCE = 80f;
	private const float MAX_HINT_DISTANCE = 20480f;

	private UI.EntityHintPanel _currentHintPanel;
	private IEntityHint _currentHint;

	private void TickEntityHints()
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

			if ( IsClient )
			{
				_currentHintPanel.UpdateHintPanel( hint.TextOnTick );
			}

			return;
		}

		DeleteHint();

		if ( IsClient )
		{
			if ( hint.ShowGlow && hint is ModelEntity model && model.IsValid() )
			{
				model.GlowColor = Color.White; // TODO: Let's let people change this in their settings.
				model.GlowActive = true;
			}

			_currentHintPanel = hint.DisplayHint( this );
			_currentHintPanel.Parent = UI.HintDisplay.Instance;
			_currentHintPanel.Enabled( true );
		}

		_currentHint = hint;
	}

	private void DeleteHint()
	{
		if ( IsClient )
		{
			if ( _currentHint != null && _currentHint is ModelEntity model && model.IsValid() )
			{
				model.GlowActive = false;
			}

			_currentHintPanel?.Delete( true );
			_currentHintPanel = null;
			UI.FullScreenHintMenu.Instance?.Close();
		}

		_currentHint = null;
	}
}
