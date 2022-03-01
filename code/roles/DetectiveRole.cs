using Sandbox;

namespace TTT;

[Library( "ttt_role_detective", Title = "Detective" )]
public class DetectiveRole : BaseRole
{
	public override void OnSelect( Player player )
	{
		base.OnSelect( player );

		if ( Host.IsServer )
		{
			player.Perks.Add( new BodyArmor() );
			player.AttachClothing( "models/detective_hat/detective_hat.vmdl" );
		}
	}

	public override void OnDeselect( Player player )
	{
		base.OnDeselect( player );

		if ( Host.IsServer )
			player.RemoveClothing();
	}

	public override void OnKilled( Player player )
	{
		base.OnKilled( player );

		var killer = player.LastAttacker as Player;

		if ( killer.IsValid() && killer.IsAlive() && killer.Team == Team.Traitors )
		{
			killer.Credits += 100;
			RPCs.ClientDisplayMessage( To.Single( killer.Client ), "You have received 100 credits for killing a Detective", Color.White );
		}
	}
}
