using System;
using Sandbox;

namespace TTT;

public partial class Game
{
	[AdminCmd( Name = "ttt_respawn", Help = "Respawns the current player or the player with the given id" )]
	public static void RespawnPlayer( int id = 0 )
	{
		var player = id == 0 ? ConsoleSystem.Caller.Pawn as Player : Entity.FindByIndex( id ) as Player;
		if ( !player.IsValid() || player.Client.GetValue( RawStrings.Spectator, false ) )
			return;

		player.Respawn();
	}

	[AdminCmd( Name = "ttt_giveitem" )]
	public static void GiveItem( string itemName )
	{
		if ( string.IsNullOrEmpty( itemName ) )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( itemName );
		if ( itemInfo is null )
		{
			Log.Error( $"{itemName} isn't a valid Item!" );
			return;
		}

		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( itemInfo.LibraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( itemInfo.LibraryName ) );
	}

	[AdminCmd( Name = "ttt_setrole" )]
	public static void SetRole( string roleName )
	{
		if ( Game.Current.Round is not InProgressRound )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var roleInfo = Asset.GetInfo<RoleInfo>( roleName );
		if ( roleInfo is null )
		{
			Log.Error( $"{roleName} isn't a valid Role!" );
			return;
		}

		player.SetRole( roleInfo.LibraryName );
	}

	[AdminCmd( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		Game.Current.ChangeRound( new PreRound() );
	}

	[ServerCmd( Name = "ttt_forcespec" )]
	public static void ToggleForceSpectator()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
	}

	[ServerCmd( Name = "ttt_rtv" )]
	public static void RockTheVote()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		if ( Game.Current.RockTheVoteClients.Contains( player.Client ) )
			return;

		Game.Current.RockTheVoteClients.Add( player.Client );
		UI.ChatBox.AddInfo( To.Everyone, $"{player.Client.Name} has rocked the vote! ({Game.Current.RockTheVoteClients.Count}/{Math.Round( Client.All.Count * 0.66 )})" );
	}
}
