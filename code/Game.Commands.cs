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
	public static void GiveItem( string libraryName )
	{
		if ( string.IsNullOrEmpty( libraryName ) )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = Asset.GetInfo<ItemInfo>( libraryName );

		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( Library.Create<Carriable>( libraryName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( Library.Create<Perk>( libraryName ) );
	}

	[AdminCmd( Name = "ttt_setrole" )]
	public static void SetRole( string roleName )
	{
		if ( Game.Current.Round is not InProgressRound )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.SetRole( roleName );
	}

	[ServerCmd( Name = "ttt_forcespec" )]
	public static void ToggleForceSpectator()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
	}

	[AdminCmd( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		Game.Current.ChangeRound( new PreRound() );
	}
}
