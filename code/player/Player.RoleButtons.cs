using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Player
{
	public List<RoleButton> RoleButtons { get; set; } = new();
	public List<UI.RoleButtonPoint> RoleButtonPoints { get; set; } = new(); // UI display
	public static RoleButton FocusedButton;
	public bool HasTrackedButtons => RoleButtons.Count > 0; // LogicButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

	public void ClearRoleButtons()
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

	// Handle client telling server to activate a specific button
	[ServerCmd]
	public static void ActivateRoleButton( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
		{
			Log.Warning( "Server received call from null player to activate logic button." );

			return;
		}

		var entity = FindByIndex( networkIdent );

		if ( entity is null || entity is not RoleButton button )
		{
			Log.Warning( $"Server received call for null logic button with network id `{networkIdent}`." );

			return;
		}

		if ( player.Role == button.Role && !button.IsDisabled )
			button.Press( player );
	}

	// Client keybinding for activating button within focus.
	public void ActivateRoleButton()
	{
		Host.AssertClient();

		if ( FocusedButton is null || !Input.Pressed( InputButton.Use ) )
			return;

		ActivateRoleButton( FocusedButton.NetworkIdent );
	}
}
