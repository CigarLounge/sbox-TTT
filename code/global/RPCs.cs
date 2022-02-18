using Sandbox;

using TTT.Events;
using TTT.Player;
using TTT.Roles;
using TTT.UI;

namespace TTT.Globals;

public partial class RPCs
{
	[ClientRpc]
	public static void ClientOnPlayerDied( TTTPlayer player )
	{
		if ( !player.IsValid() )
		{
			return;
		}

		Event.Run( TTTEvent.Player.Died, player );
	}

	[ClientRpc]
	public static void ClientOnPlayerSpawned( TTTPlayer player )
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
	public static void ClientSetRole( TTTPlayer player, string roleName )
	{
		if ( !player.IsValid() )
		{
			return;
		}

		player.SetRole( roleName );
		Scoreboard.Instance?.UpdateClient( player.Client );
	}

	// Someone refactor this mess.
	[ClientRpc]
	public static void ClientConfirmPlayer( TTTPlayer confirmPlayer, PlayerCorpse playerCorpse, TTTPlayer deadPlayer, string deadPlayerName, long deadPlayerId, string roleName, ConfirmationData confirmationData, string killerWeapon, string[] perks )
	{
		if ( !deadPlayer.IsValid() )
			return;

		deadPlayer.SetRole( roleName );
		deadPlayer.IsConfirmed = true;
		deadPlayer.CorpseConfirmer = confirmPlayer;

		if ( playerCorpse.IsValid() )
		{
			playerCorpse.DeadPlayer = deadPlayer;
			playerCorpse.KillerWeapon = Items.ItemInfo.Collection[killerWeapon] as Items.CarriableInfo;
			playerCorpse.Perks = perks;

			playerCorpse.DeadPlayerClientData = new ClientData()
			{
				Name = deadPlayerName,
				PlayerId = deadPlayerId
			};

			playerCorpse.CopyConfirmationData( confirmationData );
		}

		Client deadClient = deadPlayer.Client;

		Scoreboard.Instance.UpdateClient( deadClient );

		if ( !confirmPlayer.IsValid() )
		{
			return;
		}

		Client confirmClient = confirmPlayer.Client;

		InfoFeed.Current?.AddEntry(
			confirmClient,
			playerCorpse.DeadPlayerClientData.Name,
			deadPlayer.Role.Info.Color,
			"found the body of",
			$"({deadPlayer.Role.Info.Name})"
		);

		if ( confirmPlayer == Local.Pawn as TTTPlayer && deadPlayer.CorpseCredits > 0 )
		{
			InfoFeed.Current?.AddEntry(
				confirmClient,
				$"found $ {deadPlayer.CorpseCredits} credits!"
			);
		}
	}

	[ClientRpc]
	public static void ClientAddMissingInAction( TTTPlayer missingInActionPlayer )
	{
		if ( !missingInActionPlayer.IsValid() )
		{
			return;
		}

		missingInActionPlayer.IsMissingInAction = true;

		Scoreboard.Instance.UpdateClient( missingInActionPlayer.Client );
	}

	[ClientRpc]
	public static void ClientOpenAndSetPostRoundMenu( string winningTeam, Color winningColor )
	{
		PostRoundMenu.Instance.OpenAndSetPostRoundMenu( new PostRoundStats(
			winningRole: winningTeam,
			winningColor: winningColor
		) );
	}

	[ClientRpc]
	public static void ClientClosePostRoundMenu()
	{
		PostRoundMenu.Instance.ClosePostRoundMenu();
	}

	[ClientRpc]
	public static void ClientOpenMapSelectionMenu()
	{
		FullScreenHintMenu.Instance?.ForceOpen( new MapSelectionMenu() );
	}

	[ClientRpc]
	public static void ClientDisplayMessage( string message, Color color )
	{
		InfoFeed.Current?.AddEntry( message, color );
	}
}
