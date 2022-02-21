using Sandbox;

namespace TTT;

public partial class Game
{
	[ServerCmd( Name = "ttt_respawn", Help = "Respawns the current player or the player with the given id" )]
	public static void RespawnPlayer( int id = 0 )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "respawn" ) )
			return;

		Player player = id == 0 ? ConsoleSystem.Caller.Pawn as Player : Entity.FindByIndex( id ) as Player;
		if ( !player.IsValid() || player.Client.GetValue<bool>( RawStrings.ForcedSpectator, false ) )
			return;

		player.Respawn();
	}

	[ServerCmd( Name = "ttt_giveitem" )]
	public static void GiveItem( string libraryName )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "items" ) )
			return;

		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );
		if ( !itemInfo?.Buyable ?? true )
			return;

		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( libraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( libraryName ) );
	}

	[ServerCmd( Name = "ttt_setrole" )]
	public static void SetRole( string roleName, int id = 0 )
	{
		if ( !ConsoleSystem.Caller.HasPermission( "role" ) )
			return;

		if ( Game.Current.Round is not InProgressRound )
			return;

		Player player = id == 0 ? ConsoleSystem.Caller.Pawn as Player : Entity.FindByIndex( id ) as Player;
		if ( !player.IsValid() )
			return;

		player.SetRole( roleName );
		player.SendClientRole();
	}

	[ServerCmd( Name = "ttt_forcespec" )]
	public static void ToggleForceSpectator()
	{
		Player player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
	}

	[ServerCmd( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		if ( !ConsoleSystem.Caller.HasPermission( "restart" ) )
			return;

		Game.Current.ChangeRound( new PreRound() );

		Log.Info( $"{ConsoleSystem.Caller.Name} forced a restart." );
	}
}
