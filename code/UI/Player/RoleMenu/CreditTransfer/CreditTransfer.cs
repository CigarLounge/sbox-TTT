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
		if ( ConsoleSystem.Caller.Pawn is not Player sendingPlayer )
			return;

		var steamId = long.Parse( rawSteamId );
		var receivingPlayer = Utils.GetPlayersWhere( p => p.IsAlive() && p.Role == sendingPlayer.Role && p.SteamId == steamId ).FirstOrDefault();

		if ( receivingPlayer is null )
			return;

		sendingPlayer.Credits -= credits;
		receivingPlayer.Credits += credits;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine(
			(Game.LocalPawn as Player)?.Credits,
			_selectedPlayer?.SteamId,
			Utils.GetPlayersWhere( p => p.IsAlive() ).HashCombine( p => p.Role.GetHashCode() )
		);
	}
}
