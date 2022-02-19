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

	// Someone refactor this mess.
	[ClientRpc]
	public static void ClientConfirmPlayer( Player confirmPlayer, PlayerCorpse playerCorpse, Player deadPlayer, string deadPlayerName, long deadPlayerId, string roleName, ConfirmationData confirmationData, string killerWeapon, string[] perks )
	{
		if ( !deadPlayer.IsValid() )
			return;

		deadPlayer.SetRole( roleName );
		deadPlayer.IsConfirmed = true;
		deadPlayer.CorpseConfirmer = confirmPlayer;

		if ( playerCorpse.IsValid() )
		{
			playerCorpse.DeadPlayer = deadPlayer;
			playerCorpse.KillerWeapon = Asset.GetInfo<CarriableInfo>( killerWeapon );
			playerCorpse.Perks = perks;

			playerCorpse.DeadPlayerClientData = new ClientData()
			{
				Name = deadPlayerName,
				PlayerId = deadPlayerId
			};

			playerCorpse.CopyConfirmationData( confirmationData );
		}

		Client deadClient = deadPlayer.Client;

		UI.Scoreboard.Instance.UpdateClient( deadClient );

		if ( !confirmPlayer.IsValid() )
		{
			return;
		}

		Client confirmClient = confirmPlayer.Client;

		UI.InfoFeed.Current?.AddEntry(
			confirmClient,
			playerCorpse.DeadPlayerClientData.Name,
			deadPlayer.Role.Info.Color,
			"found the body of",
			$"({deadPlayer.Role.Info.Name})"
		);

		if ( confirmPlayer == Local.Pawn as Player && deadPlayer.CorpseCredits > 0 )
		{
			UI.InfoFeed.Current?.AddEntry(
				confirmClient,
				$"found $ {deadPlayer.CorpseCredits} credits!"
			);
		}
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
		UI.InfoFeed.Current?.AddEntry( message, color );
	}
}
