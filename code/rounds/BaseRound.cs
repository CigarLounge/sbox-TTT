using Sandbox;

namespace TTT;

public abstract partial class BaseRound : BaseNetworkable
{
	[Net]
	public TimeUntil TimeUntilRoundEnd { get; set; }

	public virtual int RoundDuration => 0;
	public virtual string RoundName => string.Empty;
	public string TimeLeftFormatted { get { return Utils.TimerString( TimeUntilRoundEnd.Relative ); } }

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

	public virtual void OnPlayerSpawn( Player player ) { }

	public virtual void OnPlayerKilled( Player player ) { }

	public virtual void OnPlayerJoin( Player player ) { }

	public virtual void OnPlayerLeave( Player player ) { }

	public virtual void OnTick() { }

	public virtual void OnSecond()
	{
		// no reason not to be called on the client
		if ( Host.IsServer && TimeUntilRoundEnd )
			OnTimeUp();
	}

	protected virtual void OnStart() { }

	protected virtual void OnFinish() { }

	protected virtual void OnTimeUp() { }
}
