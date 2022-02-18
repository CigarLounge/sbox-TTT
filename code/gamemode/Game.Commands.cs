using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using TTT.Player;
using TTT.Roles;
using TTT.Rounds;

namespace TTT.Gamemode;

public partial class Game
{
	private static TTTPlayer GetPlayerById( int id )
	{
		List<Client> playerList = Client.All.ToList();

		if ( playerList.Count <= id )
		{
			return null;
		}

		if ( playerList[id].Pawn is TTTPlayer player && player.IsValid() )
		{
			return player;
		}

		return null;
	}

	[ServerCmd( Name = "ttt_respawn", Help = "Respawns the current player or the player with the given id" )]
	public static void RespawnPlayer( string id = null )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "respawn" ) )
			return;

		TTTPlayer player = null;

		if ( id == null )
		{
			player = ConsoleSystem.Caller.Pawn as TTTPlayer;
		}
		else
		{
			try
			{
				player = GetPlayerById( int.Parse( id ) );
			}
			catch ( Exception )
			{
				return;
			}
		}

		if ( player == null || player.Client.GetValue<bool>( RawStrings.ForcedSpectator, false ) )
		{
			if ( id == null )
			{
				Log.Info( $"You tried to respawn yourself while you've been a forced spectator this round." );
			}
			else
			{
				Log.Info( $"You tried to spawn the player '{player.Client.Name}' who have been a forced spectator this round." );
			}

			return;
		}

		player.Respawn();

		if ( id == null )
		{
			Log.Info( $"You respawned yourself." );
		}
		else
		{
			Log.Info( $"You've respawned the player '{player.Client.Name}'." );
		}
	}

	[ServerCmd( Name = "ttt_requestitem" )]
	public static void RequestItem( string libraryName )
	{
		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;
		if ( !player.IsValid() )
		{
			return;
		}

		var itemInfo = Items.ItemInfo.All[libraryName] as Items.ItemInfo;
		if ( !itemInfo.Buyable )
			return;

		if ( itemInfo is Items.CarriableInfo )
			player.Inventory.Add( Library.Create<Items.Carriable>( libraryName ) );
		else if ( itemInfo is Items.PerkInfo )
			player.Perks.Add( Library.Create<Items.Perk>( libraryName ) );
	}

	[ServerCmd( Name = "ttt_giveitem" )]
	public static void GiveItem( string libraryName )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "items" ) )
			return;

		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;
		if ( !player.IsValid() )
			return;

		var itemInfo = Items.ItemInfo.All[libraryName] as Items.ItemInfo;
		if ( !itemInfo.Buyable )
			return;

		if ( itemInfo is Items.CarriableInfo )
			player.Inventory.Add( Library.Create<Items.Carriable>( libraryName ) );
		else if ( itemInfo is Items.PerkInfo )
			player.Perks.Add( Library.Create<Items.Perk>( libraryName ) );
	}

	[ServerCmd( Name = "ttt_setrole" )]
	public static void SetRole( string roleName, string id = null )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "role" ) )
		{
			return;
		}

		if ( TTT.Gamemode.Game.Current.Round is not TTT.Rounds.InProgressRound )
		{
			if ( id == null )
			{
				Log.Info( $"{ConsoleSystem.Caller.Name} tried to change his/her role when the game hadn't started." );
			}
			else
			{
				Log.Info( $"{ConsoleSystem.Caller.Name} tried to change role of ID {id} when the game hadn't started." );
			}

			return;
		}

		Type type = Utils.GetTypeByLibraryTitle<BaseRole>( roleName );

		if ( type == null )
		{
			Log.Info( $"{ConsoleSystem.Caller.Name} entered a wrong role name: '{roleName}'." );

			return;
		}

		BaseRole role = Utils.GetObjectByType<BaseRole>( type );

		if ( role == null )
		{
			return;
		}

		TTTPlayer player = null;

		if ( id == null )
		{
			player = ConsoleSystem.Caller.Pawn as TTTPlayer;
		}
		else
		{
			try
			{
				player = GetPlayerById( int.Parse( id ) );
			}
			catch ( Exception )
			{
				return;
			}
		}

		if ( player == null )
		{
			return;
		}

		player.SetRole( role );
		player.SendClientRole();
	}

	[ServerCmd( Name = "ttt_forcespec" )]
	public static void ToggleForceSpectator()
	{
		TTTPlayer player = ConsoleSystem.Caller.Pawn as TTTPlayer;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
	}

	[ServerCmd( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		if ( !ConsoleSystem.Caller.HasPermission( "restart" ) )
		{
			return;
		}

		TTT.Gamemode.Game.Current.ChangeRound( new PreRound() );

		Log.Info( $"{ConsoleSystem.Caller.Name} forced a restart." );
	}
}
