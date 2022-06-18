using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;

namespace TTT.UI;

public class RoleButtonPoint : Panel
{
	private const int CenterPercent = 50;
	private readonly float _minViewDistance = 512;
	private readonly float _maxViewDistance = 1024;
	private readonly float _focusSize = 2.5f;

	private readonly RoleButton _roleButton;

	public RoleButtonPoint( RoleButton roleButton )
	{
		_roleButton = roleButton;
		_maxViewDistance = roleButton.Radius;
		_minViewDistance = Math.Min( _minViewDistance, _maxViewDistance / 2 );

		StyleSheet.Load( "/map/rolebutton/RoleButtonPoint.scss" );
		Local.Hud.AddChild( this );

		var roleInfo = GameResource.GetInfo<RoleInfo>( _roleButton.Role );
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
		this.Enabled( !_roleButton.IsDisabled );

		Style.Left = Length.Fraction( screenPos.x );
		Style.Top = Length.Fraction( screenPos.y );
		Style.Opacity = Math.Clamp( 1f - (player.Position.Distance( _roleButton.Position ) - _minViewDistance) / (_maxViewDistance - _minViewDistance), 0f, 1f );

		if ( !this.IsEnabled() )
		{
			if ( Player.FocusedButton == _roleButton )
				Player.FocusedButton = null;
			return;
		}

		SetClass( "focus", Player.FocusedButton == _roleButton );

		if ( IsLookingAtRoleButton() && player.Position.Distance( _roleButton.Position ) <= _maxViewDistance )
			Player.FocusedButton ??= _roleButton;
		else if ( Player.FocusedButton == _roleButton )
			Player.FocusedButton = null;
	}

	public bool IsLookingAtRoleButton()
	{
		return Style.Left.Value.Value > CenterPercent - _focusSize && Style.Left.Value.Value < CenterPercent + _focusSize
			&& Style.Top.Value.Value > CenterPercent - _focusSize * Screen.Aspect && Style.Top.Value.Value < CenterPercent + _focusSize * Screen.Aspect;
	}
}
