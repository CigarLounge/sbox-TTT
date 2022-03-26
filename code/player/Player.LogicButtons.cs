using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace TTT;

public partial class Player
{
	public static Dictionary<int, LogicButtonData> LogicButtons = new();
	public static Dictionary<int, UI.LogicButtonPoint> LogicButtonPoints = new(); // UI display
	public static UI.LogicButtonPoint FocusedButton;
	public bool HasTrackedButtons => LogicButtons.Count > 0; // LogicButtons will never have a situation where a button is removed, therefore this value remains the same throughout.

	public void SendLogicButtonsToClient()
	{
		Host.AssertServer();

		List<LogicButtonData> logicButtonDataList = new();

		foreach ( Entity entity in All )
		{
			if ( entity is LogicButton logicButton && logicButton.CheckTeam == Role.Info.Team )
			{
				logicButtonDataList.Add( logicButton.PackageData() );
			}
		}

		// Network a small amount of data for each button within the player's scope.
		ClientStoreLogicButton( To.Single( this ), logicButtonDataList.ToArray() );
	}

	[Event.Hotload]
	public static void OnHotload()
	{
		if ( Host.IsClient )
			return;

		foreach ( Player player in Utils.GetPlayers() )
		{
			player.SendLogicButtonsToClient();
		}
	}

	[TTTEvent.UI.Reloaded]
	public static void OnUIReloaded()
	{
		LogicButtonPoints = new();

		foreach ( KeyValuePair<int, LogicButtonData> keyValuePair in LogicButtons )
		{
			LogicButtonPoints.Add( keyValuePair.Key, new UI.LogicButtonPoint( keyValuePair.Value ) );
		}
	}

	// Receive data of player's buttons from client.
	[ClientRpc]
	public void ClientStoreLogicButton( LogicButtonData[] buttons )
	{
		Clear();

		FocusedButton = null;

		// Index our data table by the Logic buttons network identity so we can find it later if need be.
		LogicButtons = buttons.ToDictionary( k => k.NetworkIdent, v => v );
		LogicButtonPoints = buttons.ToDictionary( k => k.NetworkIdent, v => new UI.LogicButtonPoint( v ) );
	}

	// Clear logic buttons, called before player respawns.
	[ClientRpc]
	public void RemoveLogicButtons()
	{
		Clear();
	}

	private void Clear()
	{
		foreach ( UI.LogicButtonPoint logicButtonPoint in LogicButtonPoints.Values )
		{
			logicButtonPoint.Delete( true );
		}

		LogicButtons.Clear();
		LogicButtonPoints.Clear();
		FocusedButton = null;
	}

	// Debug method
	[ServerCmd( "ttt_debug_sendrb" )]
	public static void ForceRBSend()
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;

		if ( !player.IsValid() )
			return;

		IEnumerable<LogicButton> logicButtons = All.Where( x => x is LogicButton ).Select( x => x as LogicButton );
		IEnumerable<LogicButton> applicableButtons = logicButtons.Where( x => x.CheckTeam == Team.Traitors );

		player.ClientStoreLogicButton( To.Single( player ), applicableButtons.Select( x => x.PackageData() ).ToArray() );
	}

	// Handle client telling server to activate a specific button
	[ServerCmd]
	public static void ActivateLogicButton( int networkIdent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
		{
			Log.Warning( "Server received call from null player to activate logic button." );

			return;
		}

		Entity entity = FindByIndex( networkIdent );

		if ( entity == null || entity is not LogicButton button )
		{
			Log.Warning( $"Server received call for null logic button with network id `{networkIdent}`." );

			return;
		}

		if ( button.CanUse() )
			button.Press( player );
	}

	// Client keybinding for activating button within focus.
	public void LogicButtonActivate()
	{
		if ( FocusedButton == null || !Input.Pressed( InputButton.Use ) )
			return;

		// Double check all of our data that initially set `FocusedButton` to make sure nothing has changed or any fuckery is about.
		if ( FocusedButton.IsLengthWithinCamerasFocus() && FocusedButton.IsUsable( this ) )
			ActivateLogicButton( FocusedButton.Data.NetworkIdent );
	}
}
