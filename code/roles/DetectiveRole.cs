using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Items;
using TTT.Player;
using TTT.Teams;

namespace TTT.Roles
{
	[Role( "Detective" )]
	public class DetectiveRole : TTTRole
	{
		public override Color Color => Color.FromBytes( 25, 102, 255 );
		public override int DefaultCredits => 100;
		public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( InnocentTeam ) );

		public DetectiveRole() : base()
		{

		}

		public override void OnSelect( TTTPlayer player )
		{
			if ( Host.IsServer && player.Team == DefaultTeam )
			{
				if ( player.Team == DefaultTeam )
				{
					foreach ( TTTPlayer otherPlayer in Utils.GetPlayers( ( pl ) => pl != player ) )
					{
						player.SendClientRole( To.Single( otherPlayer ) );
					}
				}

				player.Inventory.Add( new BodyArmor() );
			}

			base.OnSelect( player );
		}

		public override void OnKilled( TTTPlayer killer )
		{
			if ( killer.IsValid() && killer.LifeState == LifeState.Alive && killer.Role is TraitorRole )
			{
				killer.Credits += 100;
				RPCs.ClientDisplayMessage( To.Single( killer.Client ), "You have received 100 credits for killing a Detective", Color.White );
			}
		}

		// serverside function
		public override void CreateDefaultShop()
		{
			Shop.AddItemsForRole( this );

			base.CreateDefaultShop();
		}

		// serverside function
		public override void UpdateDefaultShop( List<Type> newItemsList )
		{
			Shop.AddNewItems( newItemsList );

			base.UpdateDefaultShop( newItemsList );
		}
	}
}
