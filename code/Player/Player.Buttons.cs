using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public static List<RoleButton> RoleButtons { get; set; } = new();
	public static List<UI.RoleButtonPoint> RoleButtonPoints { get; set; } = new();
	public static RoleButton FocusedButton { get; set; }

	public void ClearButtons()
	{
		Host.AssertClient();

		foreach ( var logicButtonPoint in RoleButtonPoints )
		{
			logicButtonPoint.Delete( true );
		}

		RoleButtons.Clear();
		RoleButtonPoints.Clear();
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

		if ( (button.Role == "All" || player.Role == button.Role) && !button.IsDisabled )
			button.Press( player );
	}

	public void ActivateRoleButton()
	{
		Host.AssertClient();

		if ( FocusedButton is null || !Input.Pressed( InputButton.Use ) )
			return;

		ActivateRoleButton( FocusedButton.NetworkIdent );
	}
}
