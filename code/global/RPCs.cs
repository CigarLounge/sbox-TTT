using Sandbox;

namespace TTT;

public partial class RPCs
{
	[ClientRpc]
	public static void ClientOnPlayerDied( Player player )
	{
		if ( !player.IsValid() )
		{
			return;
		}

		Event.Run( TTTEvent.Player.Died, player );
	}

	[ClientRpc]
	public static void ClientOnPlayerSpawned( Player player )
	{
		if ( !player.IsValid() )
		{
			return;
		}

		player.IsMissingInAction = false;
		player.IsConfirmed = false;
		player.CorpseConfirmer = null;

		player.SetRole( new NoneRole() );
	}

	[ClientRpc]
	public static void ClientSetRole( Player player, string roleName )
	{
		if ( !player.IsValid() )
		{
			return;
		}

		player.SetRole( roleName );
		UI.Scoreboard.Instance?.UpdateClient( player.Client );
	}

	[ClientRpc]
	public static void ClientAddMissingInAction( Player missingInActionPlayer )
	{
		if ( !missingInActionPlayer.IsValid() )
		{
			return;
		}

		missingInActionPlayer.IsMissingInAction = true;

		UI.Scoreboard.Instance.UpdateClient( missingInActionPlayer.Client );
	}

	[ClientRpc]
	public static void ClientOpenAndSetPostRoundMenu( string winningTeam, Color winningColor )
	{
		UI.PostRoundMenu.Instance.OpenAndSetPostRoundMenu( new UI.PostRoundStats(
			winningRole: winningTeam,
			winningColor: winningColor
		) );
	}

	[ClientRpc]
	public static void ClientClosePostRoundMenu()
	{
		UI.PostRoundMenu.Instance.ClosePostRoundMenu();
	}

	[ClientRpc]
	public static void ClientOpenMapSelectionMenu()
	{
		UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapSelectionMenu() );
	}

	[ClientRpc]
	public static void ClientDisplayMessage( string message, Color color )
	{
		UI.InfoFeed.Instance?.AddEntry( message, color );
	}
}
