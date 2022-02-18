using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTT.Events;
using TTT.Items;
using TTT.Rounds;
using TTT.UI;

namespace TTT.Player;

public partial class TTTPlayer
{
	public HashSet<string> BoughtItemsSet = new();

	[TTTEvent.Game.RoundChanged]
	private static void OnRoundChanged( BaseRound oldRound, BaseRound newRound )
	{
		if ( newRound is PreRound preRound )
		{
			foreach ( TTTPlayer player in Utils.GetPlayers() )
			{
				player.BoughtItemsSet.Clear();
			}
		}
	}

	[ClientRpc]
	public static void ClientBoughtItem( string itemName )
	{
		(Local.Pawn as TTTPlayer).BoughtItemsSet.Add( itemName );

		UpdateQuickShop();
	}

	[ClientRpc]
	public static void ClientSendQuickShopUpdate()
	{
		UpdateQuickShop();
	}

	private static void UpdateQuickShop()
	{
		// if ( QuickShop.Instance?.Enabled ?? false )
		// {
		// 	QuickShop.Instance.Update();
		// }
	}

	public void ServerUpdateShop()
	{

	}
}
