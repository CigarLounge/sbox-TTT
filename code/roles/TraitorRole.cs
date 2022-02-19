using System;
using System.Collections.Generic;

using Sandbox;

namespace TTT;

[Library( "ttt_role_traitor" )]
public class TraitorRole : BaseRole
{
	public override void OnSelect( Player player )
	{
		if ( Host.IsServer && player.Team == Team.None )
		{
			foreach ( Player otherPlayer in player.Team.GetAll() )
			{
				if ( otherPlayer == player )
				{
					continue;
				}

				player.SendClientRole( To.Single( otherPlayer ) );
				otherPlayer.SendClientRole( To.Single( player ) );
			}

			foreach ( Player otherPlayer in Utils.GetPlayers() )
			{
				if ( otherPlayer.IsMissingInAction )
				{
					otherPlayer.SyncMIA( player );
				}
			}
		}

		base.OnSelect( player );
	}

	public override void OnKilled( Player killer )
	{
		var clients = Utils.GiveAliveDetectivesCredits( 100 );
		RPCs.ClientDisplayMessage( To.Multiple( clients ), "Detectives, you have been awarded 100 equipment credits for your performance.", Color.White );
	}

	// serverside function
	public override void CreateDefaultShop()
	{
		base.CreateDefaultShop();
	}

	// serverside function
	public override void UpdateDefaultShop( List<Type> newItemsList )
	{
		base.UpdateDefaultShop( newItemsList );
	}
}
