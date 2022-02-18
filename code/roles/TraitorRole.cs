using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Player;

namespace TTT.Roles;

public class TraitorRole : BaseRole
{
	public override Team Team => Team.Traitors;
	public override string Name => "Traitor";
	public override Color Color => Color.FromBytes( 223, 41, 53 );
	public override int DefaultCredits => 200;

	public override void OnSelect( TTTPlayer player )
	{
		if ( Host.IsServer && player.Team == Team.None )
		{
			foreach ( TTTPlayer otherPlayer in player.Team.GetAll() )
			{
				if ( otherPlayer == player )
				{
					continue;
				}

				player.SendClientRole( To.Single( otherPlayer ) );
				otherPlayer.SendClientRole( To.Single( player ) );
			}

			foreach ( TTTPlayer otherPlayer in Utils.GetPlayers() )
			{
				if ( otherPlayer.IsMissingInAction )
				{
					otherPlayer.SyncMIA( player );
				}
			}
		}

		base.OnSelect( player );
	}

	public override void OnKilled( TTTPlayer killer )
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
