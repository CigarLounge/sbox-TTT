using Sandbox;

namespace TTT;

[Category( "Roles" )]
[ClassName( "ttt_role_detective" )]
[Title( "Detective" )]
public class Detective : Role
{
	public static readonly Model Hat = Model.Load( "models/detective_hat/detective_hat.vmdl" );

	public override void OnSelect( Player player )
	{
		base.OnSelect( player );

		if ( !Host.IsServer )
			return;

		player.IsRoleKnown = true;
		player.Inventory.Add( new DNAScanner() );
		player.Perks.Add( new Armor() );
		player.AttachClothing( Hat.ResourcePath );
	}

	public override void OnDeselect( Player player )
	{
		base.OnDeselect( player );

		if ( Host.IsServer )
			player.RemoveClothing( Hat.ResourcePath );
	}
}
