using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

namespace TTT;

public partial class Player
{
	public HashSet<string> BoughtItemsSet = new();

	[TTTEvent.Game.RoundChanged]
	private void OnRoundChanged( BaseRound oldRound, BaseRound newRound )
	{
		if ( newRound is not PreRound )
			return;

		BoughtItemsSet.Clear();
	}

	[ClientRpc]
	public static void ClientBoughtItem( string itemName )
	{
		(Local.Pawn as Player).BoughtItemsSet.Add( itemName );

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

	// public void TickPlayerShop()
	// {
	// 	if ( !IsClient || QuickShop.Instance == null )
	// 	{
	// 		return;
	// 	}

	// 	if ( Input.Released( InputButton.View ) )
	// 	{
	// 		QuickShop.Instance.Enabled = false;
	// 		QuickShop.Instance.Update();
	// 	}
	// 	else if ( Input.Pressed( InputButton.View ) && Local.Pawn is TTTPlayer player )
	// 	{
	// 		if ( !(player.Shop?.Accessable() ?? false) )
	// 		{
	// 			return;
	// 		}

	// 		QuickShop.Instance.Enabled = true;
	// 		QuickShop.Instance.Update();
	// 	}
	// }

}
