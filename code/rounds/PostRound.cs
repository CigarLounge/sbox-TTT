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

		bool shouldChangeMap = Game.Current.MapSelection.TotalRoundsPlayed >= Game.RoundLimit;
		Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

	public override void OnPlayerKilled( Player player )
	{
		player.MakeSpectator();
	}

	public override void OnPlayerJoin( Player playerJoined )
	{
		foreach ( Player player in Utils.GetPlayers() )
		{
			if ( player.IsRoleKnown )
			{
				player.SendClientRole( To.Single( playerJoined ) );
			}
		}
	}

	protected override void OnStart()
	{
		if ( !Host.IsServer )
			return;

		foreach ( Player player in Utils.GetPlayers() )
		{
			if ( player.Corpse.IsValid() && !player.IsConfirmedDead )
				player.Corpse.Confirm();
			else
				player.SendClientRole( To.Everyone );
		}
	}
}
