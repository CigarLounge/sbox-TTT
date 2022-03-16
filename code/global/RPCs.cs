using Sandbox;

namespace TTT;

public partial class RPCs
{
	[ClientRpc]
	public static void ClientOnPlayerDied( Player player )
	{
		if ( !player.IsValid() )
			return;

		Event.Run( TTTEvent.Player.Killed, player );
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
	public static void ClientDisplayEntry( string message, Color color )
	{
		UI.InfoFeed.Instance?.AddEntry( message, color );
	}

	[ClientRpc]
	public static void ClientDisplayClientEntry( string message )
	{
		UI.InfoFeed.Instance?.AddClientEntry( Local.Client, message );
	}

	[ClientRpc]
	public static void ClientDisplayRoleEntry( int roleId, string message )
	{
		UI.InfoFeed.Instance?.AddRoleEntry( roleId, message );
	}
}
