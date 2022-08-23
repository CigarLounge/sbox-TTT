using System;
using Sandbox;

namespace TTT;

public partial class Game
{
	[ConCmd.Admin( Name = "ttt_respawn", Help = "Respawns the current player or the player with the given id" )]
	public static void RespawnPlayer( int id = 0 )
	{
		var player = id == 0 ? ConsoleSystem.Caller.Pawn as Player : Entity.FindByIndex( id ) as Player;
		if ( !player.IsValid() )
			return;

		if ( player.IsForcedSpectator )
			player.ToggleForcedSpectator();

		player.Respawn();
	}

	[ConCmd.Admin( Name = "ttt_giveitem" )]
	public static void GiveItem( string itemName )
	{
		if ( itemName.IsNullOrEmpty() )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var itemInfo = GameResource.GetInfo<ItemInfo>( itemName );
		if ( itemInfo is null )
		{
			Log.Error( $"{itemName} isn't a valid Item!" );
			return;
		}

		if ( itemInfo is CarriableInfo )
			player.Inventory.Add( TypeLibrary.Create<Carriable>( itemInfo.ClassName ) );
		else if ( itemInfo is PerkInfo )
			player.Perks.Add( TypeLibrary.Create<Perk>( itemInfo.ClassName ) );
	}

	[ConCmd.Admin( Name = "ttt_givecredits" )]
	public static void GiveCredits( int credits )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.Credits += credits;
	}

	[ConCmd.Admin( Name = "ttt_setrole" )]
	public static void SetRole( string roleName )
	{
		if ( Game.Current.State is not InProgress )
			return;

		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		var roleInfo = GameResource.GetInfo<RoleInfo>( roleName );
		if ( roleInfo is null )
		{
			Log.Error( $"{roleName} isn't a valid Role!" );
			return;
		}

		player.SetRole( roleInfo );
	}

	[ConCmd.Admin( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		Game.Current.ChangeState( new PreRound() );
	}

	[ConCmd.Server( Name = "ttt_forcespec" )]
	public static void ToggleForceSpectator()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
	}

	[ConCmd.Server( Name = "ttt_togglemute" )]
	public static void ToggleMute()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.PlayersMuted = player.PlayersMuted switch
		{
			PlayersMute.None => PlayersMute.AlivePlayers,
			PlayersMute.AlivePlayers => PlayersMute.Spectators,
			PlayersMute.Spectators => PlayersMute.All,
			PlayersMute.All => PlayersMute.None,
			_ => PlayersMute.None
		};
	}

	[ConCmd.Server( Name = "ttt_rtv" )]
	public static void RockTheVote()
	{
		var client = ConsoleSystem.Caller;
		if ( !client.IsValid() )
			return;

		if ( client.GetValue<bool>( Strings.HasRockedTheVote ) )
			return;

		client.SetValue( Strings.HasRockedTheVote, true );
		Game.Current.RTVCount += 1;

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has rocked the vote! ({Game.Current.RTVCount}/{MathF.Round( Client.All.Count * Game.RTVThreshold )})" );
	}
}
