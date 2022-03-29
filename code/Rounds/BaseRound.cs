using Sandbox;

namespace TTT;

public abstract partial class BaseRound : BaseNetworkable
{
	[Net]
	public TimeUntil TimeUntilRoundEnd { get; set; }

	public virtual int RoundDuration => 0;
	public virtual string RoundName => string.Empty;
	public string TimeLeftFormatted => TimeUntilRoundEnd.Relative.TimerString();
	private RealTimeUntil _nextSecondTime = 0f;

	public void Start()
	{
		if ( Host.IsServer && RoundDuration > 0 )
			TimeUntilRoundEnd = RoundDuration;

		OnStart();
	}

	public void Finish()
	{
		if ( Host.IsServer )
			TimeUntilRoundEnd = 0f;

		OnFinish();
	}

	public virtual void OnPlayerSpawned( Player player ) { }

	public virtual void OnPlayerKilled( Player player ) { }

	public virtual void OnPlayerJoin( Player player ) { }

	public virtual void OnPlayerLeave( Player player ) { }

	public virtual void OnTick()
	{
		if ( _nextSecondTime )
		{
			OnSecond();
			_nextSecondTime = 1f;
		}
	}

	public virtual void OnSecond()
	{
		if ( Host.IsServer && TimeUntilRoundEnd )
			OnTimeUp();
	}

	protected virtual void OnStart() { }

	protected virtual void OnFinish() { }

	protected virtual void OnTimeUp() { }

	protected void RevealEveryone()
	{
		Host.AssertServer();

		foreach ( var client in Client.All )
		{
			var player = client.Pawn as Player;

			if ( !player.IsAlive() && !player.IsConfirmedDead )
				player.Confirm();
			else if ( !player.IsRoleKnown )
				player.SendRoleToClient( To.Everyone );

			player.IsRoleKnown = true;
		}
	}

	protected void SyncPlayer( Player player )
	{
		Host.AssertServer();

		foreach ( var client in Client.All )
		{
			var otherPlayer = client.Pawn as Player;

			if ( otherPlayer.IsConfirmedDead )
				otherPlayer.Confirm( To.Single( player ) );
			else if ( otherPlayer.IsRoleKnown )
				otherPlayer.SendRoleToClient( To.Single( player ) );
		}
	}
}
