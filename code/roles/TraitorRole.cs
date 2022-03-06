using Sandbox;

namespace TTT;

[Library( "ttt_role_traitor", Title = "Traitor" )]
public class TraitorRole : BaseRole
{
	public override void OnSelect( Player player )
	{
		base.OnSelect( player );

		if ( Host.IsServer )
		{
			foreach ( Player otherPlayer in player.Team.GetAll() )
			{
				if ( otherPlayer == player )
					continue;

				player.SendRoleToClient( To.Single( otherPlayer ) );
				otherPlayer.SendRoleToClient( To.Single( player ) );
			}

			foreach ( Player otherPlayer in Utils.GetPlayers() )
			{
				if ( otherPlayer.IsMissingInAction )
				{
					otherPlayer.SyncMIA( player );
				}
			}
		}
	}

	public override void OnKilled( Player player )
	{
		base.OnKilled( player );

		var clients = Utils.GiveAliveDetectivesCredits( 100 );
		RPCs.ClientDisplayMessage( To.Multiple( clients ), "Detectives, you have been awarded 100 equipment credits for your performance.", Color.White );
	}
}
