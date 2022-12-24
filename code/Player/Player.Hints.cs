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
		if ( hint is null || !hint.CanHint( CameraMode.Target ) )
		{
			DeleteHint();
			return;
		}

		if ( hint == _currentHint )
		{
			hint.Tick( CameraMode.Target );
			return;
		}

		DeleteHint();

		_currentHintPanel = hint.DisplayHint( CameraMode.Target );
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
			.Ignore( CameraMode.Target )
			.WithAnyTags( "solid", "interactable" )
			.UseHitboxes()
			.Run();

		if ( HoveredEntity is IEntityHint oldHint && oldHint.ShowGlow )
			if ( HoveredEntity.Components.TryGet<Glow>( out var activeGlow ) )
				activeGlow.Enabled = false;

		HoveredEntity = trace.Entity;

		if ( HoveredEntity is IEntityHint newHint )
		{
			if ( trace.Distance <= newHint.HintDistance )
			{
				if ( newHint.ShowGlow )
				{
					var glow = HoveredEntity.Components.GetOrCreate<Glow>();
					glow.Width = 0.25f;
					glow.Color = Role.Color;
					glow.Enabled = true;
				}
			}

			return newHint;
		}

		return null;
	}
}
