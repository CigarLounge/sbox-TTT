using System;
using Sandbox;

namespace TTT;

public class PostRound : BaseRound
{
	public override string RoundName => "Post";
	public override int RoundDuration => Game.PostRoundTime;

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		player.Confirm( To.Everyone );
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

		// Add the score gained this round to the actual score.
		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			player.Score += player.RoundScore;
			player.RoundScore = 0;
		}

		Karma.OnRoundEnd();
	}

	protected override void OnFinish()
	{
		base.OnFinish();

		if ( Host.IsClient )
			UI.PostRoundMenu.Instance?.Close();
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		bool shouldChangeMap = Game.Current.TotalRoundsPlayed >= Game.RoundLimit || Game.Current.RTVCount >= MathF.Round( Client.All.Count * Game.RTVThreshold );
		Game.Current.ChangeRound( shouldChangeMap ? new MapSelectionRound() : new PreRound() );
	}

}
