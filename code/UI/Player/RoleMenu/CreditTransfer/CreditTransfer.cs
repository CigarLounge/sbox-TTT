using System;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class CreditTransfer : Panel
{
	private const int CreditAmount = 100;
	private Player _selectedPlayer;

	[ConCmd.Server]
	public static void SendCredits( string rawSteamId, int credits )
	{
		var steamId = long.Parse( rawSteamId );
		var sendingPlayer = ConsoleSystem.Caller.Pawn as Player;
		if ( !sendingPlayer.IsValid() )
			return;

		var client = Game.Clients.FirstOrDefault( c => c.SteamId == steamId );
		if ( client is not null && client.Pawn is Player player && player.IsAlive() )
		{
			sendingPlayer.Credits -= credits;
			player.Credits += credits;
		}
	}

	protected override int BuildHash()
	{
		var player = Game.LocalPawn as Player;
		return HashCode.Combine( player.Credits, _selectedPlayer?.SteamId );
	}
}
