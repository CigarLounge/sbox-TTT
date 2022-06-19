using Sandbox;

namespace TTT;

[Category( "Roles" )]
[ClassName( "ttt_role_detective" )]
[Title( "Detective" )]
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

		player.Inventory.Add( new DNAScanner() );
		player.Perks.Add( new BodyArmor() );
		player.AttachClothing( "models/detective_hat/detective_hat.vmdl" );
	}

	public override void OnDeselect( Player player )
	{
		base.OnDeselect( player );

		if ( Host.IsServer )
			player.RemoveClothing( "models/detective_hat/detective_hat.vmdl" );
	}
}
