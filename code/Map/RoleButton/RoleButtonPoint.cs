using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

public class RoleButtonPoint : Panel
{
	// Our specific assigned Entity.
	private readonly RoleButton _roleButton;

	// If the distance from the player to the button is less than this value, the element is fully visible.
	private int _minViewDistance = 512;

	// Between MINVIEWDISTANCE and this value, the element will slowly become transparent.
	// Past this distance, the button is unusuable.
	private readonly int _maxViewDistance = 1024;

	public RoleButtonPoint( RoleButton roleButton )
	{
		_roleButton = roleButton;
		_maxViewDistance = roleButton.Range;
		_minViewDistance = Math.Min( _minViewDistance, _maxViewDistance / 2 );

		StyleSheet.Load( "/map/rolebutton/RoleButtonPoint.scss" );
		Local.Hud.AddChild( this );

		var roleInfo = Asset.GetInfo<RoleInfo>( _roleButton.Role );
		var pointer = Add.Label( "ads_click", "pointer" );

		if ( roleInfo is not null )
		{
			pointer.Style.TextStrokeColor = roleInfo.Color;
			pointer.Style.TextStrokeWidth = 2f;
		}

		Add.Label( _roleButton.Description, "text-shadow description" );
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;
		var screenPos = _roleButton.Position.ToScreen();
		this.Enabled( screenPos.z > 0f );

		// If our entity is locked, delayed or removed, let's not show it.
		if ( _roleButton.IsDisabled )
		{
			Style.Display = DisplayMode.None;

			// Make sure our client is no longer tracking this element.
			if ( Player.FocusedButton == _roleButton )
				Player.FocusedButton = null;

			this.Enabled( false );

			return;
		}

		if ( !this.IsEnabled() )
			return;

		Style.Display = DisplayMode.Flex;
		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
		Style.Opacity = Math.Clamp( 1f - (player.Position.Distance( _roleButton.Position ) - _minViewDistance) / (_maxViewDistance - _minViewDistance), 0f, 1f );

		// Update our 'focus' CSS look if our player currently is looking near this point.
		SetClass( "focus", Player.FocusedButton == _roleButton );

		// Check if point is within 10% of the crosshair.
		if ( IsLengthWithinCamerasFocus() && player.Position.Distance( _roleButton.Position ) <= _maxViewDistance )
		{
			Player.FocusedButton ??= _roleButton; // If the current focused button is null, update it to this.
		}
		else if ( Player.FocusedButton == _roleButton ) // If we are the current focused button, but we are out of focus, set to null
		{
			Player.FocusedButton = null;
		}
	}

	// Our "screen focus" size, roughly %5 of the screen around the cross hair.
	// It might be worth considering using an alternate method to percentages for larger screens. Hoping we can test that with someone who has a UHD monitor.
	private float _focusSize = 2.5f;
	private const int _centerPercent = 50;

	public bool IsLengthWithinCamerasFocus()
	{
		// We have to adjust the top check by the screen's aspect ratio in order to compensate for screen size
		float topHeight = _focusSize * Screen.Aspect;

		// I think we could alternatively use
		return Style.Left.Value.Value > _centerPercent - _focusSize && Style.Left.Value.Value < _centerPercent + _focusSize
			&& Style.Top.Value.Value > _centerPercent - topHeight && Style.Top.Value.Value < _centerPercent + topHeight;
	}

	// Check to make sure player is within range and our button is not disabled.
	// Called when client calls for button to be activated. A simple double check.
	public bool IsUsable( Player player )
	{
		return player.Position.Distance( _roleButton.Position ) <= _roleButton.Range && !_roleButton.IsDisabled;
	}
}
