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
			foreach ( var client in Client.All )
			{
				if ( client == player.Client )
					continue;

				var otherPlayer = client.Pawn as Player;

				if ( otherPlayer.Team == Team.Traitors )
				{
					player.SendRoleToClient( To.Single( otherPlayer ) );
					otherPlayer.SendRoleToClient( To.Single( player ) );
				}

				if ( otherPlayer.IsMissingInAction )
					otherPlayer.SyncMIA( player );
			}
		}
	}

	public override void OnKilled( Player player )
	{
		base.OnKilled( player );

		var clients = Utils.GiveAliveDetectivesCredits( 100 );
		RPCs.ClientDisplayRoleEntry( To.Multiple( clients ), new DetectiveRole().Info.Id, "you have been awarded 100 credits for your performance." );
	}
}
