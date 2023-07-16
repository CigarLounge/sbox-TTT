using System;
using Sandbox;

namespace TTT;

public enum WinType
{
	TimeUp,
	Elimination,
	Objective,
}

public partial class PostRound : BaseState
{
	[Net]
	public Team WinningTeam { get; private set; }

	[Net]
	public WinType WinType { get; private set; }

	public override string Name { get; } = "Post";
	public override int Duration => GameManager.PostRoundTime;

	public PostRound() { }

	public PostRound( Team winningTeam, WinType winType )
	{
		RevealEveryone();

		WinningTeam = winningTeam;
		WinType = winType;
	}

	public static void Load( Team winningTeam, WinType winType )
	{
		GameManager.Current.ForceStateChange( new PostRound( winningTeam, winType ) );
	}

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		player.Reveal();
	}

	public override void OnPlayerJoin( Player player )
	{
		base.OnPlayerJoin( player );

		player.Status = PlayerStatus.Spectator;
		player.UpdateStatus( To.Everyone );
	}

	protected override void OnStart()
	{
		GameManager.Current.TotalRoundsPlayed++;
		Event.Run( TTTEvent.Round.End, WinningTeam, WinType );
	}

	protected override void OnTimeUp()
	{
		GameManager.Current.ChangeState( ShouldSelectMap() ? new MapSelectionState() : new PreRound() );
	}

	private bool ShouldSelectMap()
	{
		return GameManager.Current.TotalRoundsPlayed >= GameManager.RoundLimit
				|| GameManager.Current.TimeUntilMapSwitch
				|| GameManager.Current.RTVCount >= MathF.Round( Game.Clients.Count * GameManager.RTVThreshold );
	}
}
