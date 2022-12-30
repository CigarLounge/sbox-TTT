using Sandbox;
using System;

namespace TTT;

public partial class GameManager
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

	[ConCmd.Admin( Name = "ttt_givedamage" )]
	public static void GiveDamage( float damage )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.TakeDamage( ExtendedDamageInfo.Generic( damage ) );
	}

	[ConCmd.Admin( Name = "ttt_setrole" )]
	public static void SetRole( string roleName )
	{
		if ( GameManager.Current.State is not InProgress )
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

	[ConCmd.Admin( Name = "ttt_setkarma" )]
	public static void SetKarma( int karma )
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ActiveKarma = karma;
	}

	[ConCmd.Admin( Name = "ttt_force_restart" )]
	public static void ForceRestart()
	{
		GameManager.Current.ChangeState( new PreRound() );
	}

	[ConCmd.Admin( Name = "ttt_change_map" )]
	public static async void ChangeMap( string mapIdent )
	{
		var package = await Package.Fetch( mapIdent, true );
		if ( package is not null && package.PackageType == Package.Type.Map )
			Game.ChangeLevel( mapIdent );
		else
			Log.Error( $"{mapIdent} does not exist as a s&box map!" );
	}

	[ConCmd.Server( Name = "ttt_force_spectator" )]
	public static void ToggleForceSpectator()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.ToggleForcedSpectator();
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
		GameManager.Current.RTVCount += 1;

		UI.TextChat.AddInfo( To.Everyone, $"{client.Name} has rocked the vote! ({GameManager.Current.RTVCount}/{MathF.Round( Game.Clients.Count * GameManager.RTVThreshold )})" );
	}

	[ConCmd.Server( Name = "kill" )]
	public static void Kill()
	{
		var player = ConsoleSystem.Caller.Pawn as Player;
		if ( !player.IsValid() )
			return;

		player.Kill();
	}
}
