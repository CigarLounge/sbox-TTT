using Sandbox;

namespace TTT;

public class PostRound : BaseRound
{
	public override string RoundName => "Post";
	public override int RoundDuration => Game.PostRoundTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		RPCs.ClientClosePostRoundMenu();

		bool shouldChangeMap = Game.Current.TotalRoundsPlayed >= Game.RoundLimit;
		Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		player.MakeSpectator();
		player.Confirm();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		foreach ( var client in Client.All )
		{
			var otherPlayer = client.Pawn as Player;

			if ( otherPlayer.IsConfirmedDead )
				otherPlayer.Confirm( To.Single( player ) );
			else if ( otherPlayer.IsRoleKnown )
				otherPlayer.SendRoleToClient( To.Single( player ) );
		}
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( !Host.IsServer )
			return;

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( !player.IsAlive() && !player.IsConfirmedDead )
				player.Confirm();
			else if ( !player.IsRoleKnown )
				player.SendRoleToClient( To.Everyone );
		}
	}
}
