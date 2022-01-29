using System;
using System.Collections.Generic;

using Sandbox;

using TTT.Globals;
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
				foreach ( TTTPlayer otherPlayer in Utils.GetPlayers( ( pl ) => pl != player ) )
				{
					player.SendClientRole( To.Single( otherPlayer ) );
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
