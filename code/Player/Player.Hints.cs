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
		var hint = FindHintableEntity();
		if ( hint is null || !hint.CanHint( UI.Hud.DisplayedPlayer ) )
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

		if ( _currentHint is Entity ent && _currentHint.ShowGlow )
		{
			var glow = ent.Components.GetOrCreate<Glow>();
			glow.Width = 0.25f;
			glow.Color = Role.Color;
			glow.Enabled = true;
		}
	}

	private static void DeleteHint()
	{
		_currentHintPanel?.Delete( true );
		_currentHintPanel = null;
		UI.FullScreenHintMenu.Instance?.Close();

		if ( _currentHint is Entity ent && _currentHint.ShowGlow )
			if ( ent.Components.TryGet<Glow>( out var activeGlow ) )
				activeGlow.Enabled = false;

		_currentHint = null;
	}

	private IEntityHint FindHintableEntity()
	{
		var trace = Trace.Ray( Camera.Position, Camera.Position + Camera.Rotation.Forward * MaxHintDistance )
			.Ignore( UI.Hud.DisplayedPlayer )
			.WithAnyTags( "solid", "interactable" )
			.UseHitboxes()
			.Run();

		HoveredEntity = trace.Entity;

		if ( HoveredEntity is IEntityHint hint && trace.Distance <= hint.HintDistance )
			return hint;

		return null;
	}
}
