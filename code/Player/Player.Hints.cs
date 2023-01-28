using Sandbox;
using Sandbox.Component;
using Sandbox.UI;

namespace TTT;

public partial class Player
{
	public const float MaxHintDistance = 5000f;

	private static Panel _currentHintPanel;
	private static IEntityHint _currentHint;
	private static Glow _glow;

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
			if ( _currentHint is Entity ee && _currentHint.ShowGlow )
			{
				_glow = ee.Components.GetOrCreate<Glow>();
				_glow.Width = 0.25f;
				_glow.Color = Role.Color;
				_glow.Enabled = CanUse( ee );
			}
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

		if ( _currentHint is Entity entity && _currentHint.ShowGlow )
		{
			if ( entity.Components.TryGet<Glow>( out var activeGlow ) )
				activeGlow.Enabled = false;
		}

		_glow = null;
		_currentHint = null;
	}
}
