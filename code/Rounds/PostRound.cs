using System;
using Sandbox;

namespace TTT;

public class PostRound : BaseRound
{
	public override string RoundName => "Post";
	public override int RoundDuration => Game.PostRoundTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		bool shouldChangeMap = Game.Current.TotalRoundsPlayed >= Game.RoundLimit || Game.Current.RockTheVoteClients.Count >= Math.Round( Client.All.Count * Game.RTVThreshold );
		Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		player.Confirm();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		SyncPlayer( player );
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( !Host.IsServer )
			return;

		RevealEveryone();
	}

	protected override void OnFinish()
	{
		base.OnFinish();

		if ( Host.IsClient )
			UI.PostRoundMenu.Instance.Close();
	}
}
