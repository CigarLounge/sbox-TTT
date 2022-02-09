using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Globals;
using TTT.Player;
using TTT.Teams;

namespace TTT.Roles
{
	[Role( "Traitor" )]
	public class TraitorRole : TTTRole
	{
		public override Color Color => Color.FromBytes( 223, 41, 53 );

		public override int DefaultCredits => 100;

		public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( TraitorTeam ) );

		public TraitorRole() : base()
		{

		}

		public override void OnSelect( TTTPlayer player )
		{
			if ( Host.IsServer && player.Team == DefaultTeam )
			{
				foreach ( TTTPlayer otherPlayer in player.Team.Members )
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
