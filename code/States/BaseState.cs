using Sandbox;

namespace TTT;

public abstract partial class BaseState : BaseNetworkable
{
	[Net]
	public TimeUntil TimeLeft { get; protected set; }

	public virtual int Duration => 0;
	public virtual string Name => string.Empty;
	public string TimeLeftFormatted => TimeLeft.Relative.TimerString();

	private TimeUntil _nextSecondTime = 0f;

	public void Start()
	{
		if ( Host.IsServer && Duration > 0 )
			TimeLeft = Duration;

		OnStart();
	}

	public void Finish()
	{
		if ( Host.IsServer )
			TimeLeft = 0f;

		OnFinish();
	}

	public virtual void OnPlayerSpawned( Player player )
	{
		Game.Current.MoveToSpawnpoint( player );
	}

	public virtual void OnPlayerKilled( Player player )
	{
		player.MakeSpectator();
	}

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
		if ( Host.IsServer && TimeLeft )
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

			if ( !player.IsAlive() && !player.IsConfirmedDead && !player.IsSpectator )
				player.Confirm( To.Everyone );
			else if ( !player.IsRoleKnown )
				player.SendRole( To.Everyone );

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
				otherPlayer.SendRole( To.Single( player ) );
		}
	}

	protected async void StartRespawnTimer( Player player )
	{
		await GameTask.DelaySeconds( 1 );

		if ( player.IsValid() && Game.Current.Round == this )
			player.Respawn();
	}
}
