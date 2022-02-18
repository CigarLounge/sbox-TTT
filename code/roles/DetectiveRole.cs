using System;
using System.Collections.Generic;
using Sandbox;
using TTT.Items;
using TTT.Player;

namespace TTT.Roles;

[Library( "ttt_role_detective" )]
public class DetectiveRole : BaseRole
{
	public override void OnSelect( TTTPlayer player )
	{
		if ( !Host.IsServer || player.Team != Info.Team )
			return;

		if ( player.Team == Info.Team )
		{
			foreach ( TTTPlayer otherPlayer in Utils.GetPlayers( ( pl ) => pl != player ) )
			{
				player.SendClientRole( To.Single( otherPlayer ) );
			}
		}

		player.Perks.Add( new BodyArmor() );
		player.AttachClothing( "models/detective_hat/detective_hat.vmdl" );


		base.OnSelect( player );
	}

	public override void OnKilled( TTTPlayer killer )
	{
		if ( killer.IsValid() && killer.LifeState == LifeState.Alive && killer.Role.Info.Team == Team.Traitors )
		{
			killer.Credits += 100;
			RPCs.ClientDisplayMessage( To.Single( killer.Client ), "You have received 100 credits for killing a Detective", Color.White );
		}
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
