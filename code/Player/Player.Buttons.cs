using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public static List<RoleButton> RoleButtons { get; set; } = new();
	public static List<UI.RoleButtonMarker> RoleButtonMarkers { get; set; } = new();
	public static RoleButton FocusedButton { get; set; }

	public void ClearButtons()
	{
		Game.AssertClient();

		foreach ( var logicButtonPoint in RoleButtonMarkers )
		{
			logicButtonPoint.Delete( true );
		}

		RoleButtons.Clear();
		RoleButtonMarkers.Clear();
		FocusedButton = null;
	}

	[ConCmd.Server]
	public static void ActivateRoleButton( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		var entity = FindByIndex( networkIdent );

		if ( entity is not RoleButton button )
			return;

		if ( button.CanUse( player ) )
			button.Press( player );
	}

	public void ActivateRoleButton()
	{
		Game.AssertClient();

		if ( FocusedButton is null || !Input.Pressed( InputAction.Use ) )
			return;

		ActivateRoleButton( FocusedButton.NetworkIdent );
	}
}
