using Sandbox;

namespace TTT;

[Library( "ttt_role_detective", Title = "Detective" )]
public class Detective : BaseRole
{
	public override void OnSelect( Player player )
	{
		base.OnSelect( player );

		if ( !Host.IsServer )
			return;

		player.IsRoleKnown = true;

		foreach ( var client in Client.All )
		{
			if ( client == player.Client )
				continue;

			player.SendRole( To.Single( client ) );
		}

		player.Perks.Add( new BodyArmor() );
		player.AttachClothing( "models/detective_hat/detective_hat.vmdl" );
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
			RPCs.ClientDisplayClientEntry( To.Single( killer.Client ), "have received 100 credits for killing a Detective" );
		}
	}
}
