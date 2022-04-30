using System;
using Sandbox;

namespace TTT;

public enum WinType
{
	TimeUp,
	Elimination,
	Objective,
}

public class PostRound : BaseState
{
	public Team WinningTeam { get; init; }
	public WinType WinType { get; init; }

	public override string Name => "Post";
	public override int Duration => Game.PostRoundTime;

	public PostRound() { }

	public PostRound( Team winningTeam, WinType winType )
	{
		WinningTeam = winningTeam;
		WinType = winType;
	}

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

		Event.Run( TTTEvent.Round.Ended, WinningTeam, WinType );

		if ( !Host.IsServer )
			return;

		Game.Current.TotalRoundsPlayed++;
		RevealEveryone();
	}

	protected override void OnFinish()
	{
		base.OnFinish();

		if ( Host.IsClient )
			UI.PostRoundPopup.Instance?.Close();
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		bool shouldChangeMap = Game.Current.TotalRoundsPlayed >= Game.RoundLimit || Game.Current.RTVCount >= MathF.Round( Client.All.Count * Game.RTVThreshold );
		Game.Current.ChangeState( shouldChangeMap ? new MapSelectionState() : new PreRound() );
	}

}
