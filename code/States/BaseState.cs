using Sandbox;

namespace TTT;

public abstract partial class BaseState : BaseNetworkable
{
	[Net]
	public TimeUntil TimeLeft { get; protected set; }

	[Net]
	public bool HasStarted { get; private set; }

	public virtual int Duration => 0;
	public virtual string Name { get; }
	public string TimeLeftFormatted => TimeLeft.Relative.TimerString();

	private TimeUntil _nextSecondTime = 0f;

	public void Start()
	{
		if ( Game.IsServer && Duration > 0 )
			TimeLeft = Duration;

		OnStart();
	}

	public void Finish()
	{
		if ( Game.IsServer )
			TimeLeft = 0f;

		OnFinish();
	}

	public virtual void OnPlayerSpawned( Player player )
	{
		TTTGame.Current.MoveToSpawnpoint( player );
	}

	public virtual void OnPlayerKilled( Player player )
	{
		player.MakeSpectator();
	}

	public virtual void OnPlayerJoin( Player player )
	{
		HasStarted = true;
	}

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
		if ( Game.IsServer && TimeLeft )
			OnTimeUp();
	}

	protected virtual void OnStart() { }

	protected virtual void OnFinish() { }

	protected virtual void OnTimeUp() { }

	protected static void RevealEveryone()
	{
		Game.AssertServer();

		foreach ( var client in Game.Clients )
		{
			var player = client.Pawn as Player;
			player.Reveal();
		}
	}

	protected async void StartRespawnTimer( Player player )
	{
		await GameTask.DelaySeconds( 1 );

		if ( player.IsValid() && TTTGame.Current.State == this )
			player.Respawn();
	}
}
