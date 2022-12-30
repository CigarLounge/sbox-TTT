using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class CreditTransfer : Panel
{
	private const int CreditAmount = 100;
	private Player _selectedPlayer;

	protected override int BuildHash()
	{
		return HashCode.Combine( _selectedPlayer?.SteamId );
	}
}
