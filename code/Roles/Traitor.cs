using Sandbox;

namespace TTT;

/// <summary>
/// You should be inheriting all your custom traitor roles from this.
/// </summary>
[Category( "Roles" )]
[ClassName( "ttt_role_traitor" )]
[Title( "Traitor" )]
public class Traitor : BaseRole
{
	public override void OnSelect( Player player )
	{
		base.OnSelect( player );

		if ( !Host.IsServer )
			return;

		foreach ( var client in Client.All )
		{
			if ( client == player.Client )
				continue;

			var otherPlayer = client.Pawn as Player;

			if ( otherPlayer.Team == Team )
			{
				player.SendRole( To.Single( otherPlayer ) );
				otherPlayer.SendRole( To.Single( player ) );
			}

			if ( otherPlayer.IsMissingInAction )
				otherPlayer.UpdateMissingInAction( player );
		}
	}

	protected override bool ShouldCreateRolePlate( Player player )
	{
		var local = Local.Pawn as Player;

		return local.Team == Team;
	}
}
