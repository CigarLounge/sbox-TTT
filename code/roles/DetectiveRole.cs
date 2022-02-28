using Sandbox;

namespace TTT;

[Library( "ttt_role_detective", Title = "Detective" )]
public class DetectiveRole : BaseRole
{
	public override void OnSelect( Player player )
	{
		if ( Host.IsServer )
		{
			foreach ( Player otherPlayer in Utils.GetPlayers( ( pl ) => pl != player ) )
			{
				player.SendClientRole( To.Single( otherPlayer ) );
			}

			player.Perks.Add( new BodyArmor() );
			player.AttachClothing( "models/detective_hat/detective_hat.vmdl" );
		}

		base.OnSelect( player );
	}

	public override void OnDeselect( Player player )
	{
		if ( Host.IsServer )
			player.RemoveClothing();

		base.OnSelect( player );
	}

	public override void OnKilled( Player killer )
	{
		if ( killer.IsValid() && killer.IsAlive() && killer.Team == Team.Traitors )
		{
			killer.Credits += 100;
			RPCs.ClientDisplayMessage( To.Single( killer.Client ), "You have received 100 credits for killing a Detective", Color.White );
		}
	}
}
