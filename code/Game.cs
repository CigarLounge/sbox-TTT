using Sandbox;
using System.Linq;

namespace TTT;

public partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net, Change]
	public BaseState State { get; private set; }
	private BaseState _lastState;

	[Net]
	public int TotalRoundsPlayed { get; set; }

	public int RTVCount { get; set; }

	public Game()
	{
		if ( IsServer )
			_ = new UI.Hud();
	}

	/// <summary>
	/// Changes the state if minimum players is met. Otherwise, force changes to "WaitingState"
	/// </summary>
	/// <param name="state"> The state to change to if minimum players is met.</param>
	public void ChangeState( BaseState state )
	{
		Assert.NotNull( state );

		ForceStateChange( Utils.HasMinimumPlayers() ? state : new WaitingState() );
	}

	/// <summary>
	/// Force changes a state regardless of player count.
	/// </summary>
	/// <param name="state"> The state to change to.</param>
	public void ForceStateChange( BaseState state )
	{
		Host.AssertServer();

		State?.Finish();
		var oldState = State;
		State = state;
		State.Start();

		Event.Run( TTTEvent.Game.StateChanged, oldState, State );
	}

	public override void OnKilled( Entity pawn )
	{
		// Do nothing. Base implementation just adds to a kill feed and prints to console.
	}

	public override void ClientJoined( Client client )
	{
		var player = new Player();
		client.Pawn = player;

		player.BaseKarma = Karma.DefaultValue;
		player.ActiveKarma = player.BaseKarma;

		player.IsSpectator = true;

		State.OnPlayerJoin( player );

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has joined" );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		State.OnPlayerLeave( client.Pawn as Player );

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has left ({reason})" );

		// Only delete the pawn if they are alive.
		// Keep the dead body otherwise on disconnect.
		if ( client.Pawn.IsValid() && client.Pawn.IsAlive() )
			client.Pawn.Delete();

		client.Pawn = null;
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		if ( !source.Pawn.IsAlive() && !dest.Pawn.IsAlive() )
			return true;

		if ( State is InProgress && !source.Pawn.IsAlive() && dest.Pawn.IsAlive() )
			return false;

		return true;
	}

	public override void OnVoicePlayed( long playerId, float level )
	{
		var client = Client.All.Where( x => x.PlayerId == playerId ).FirstOrDefault();
		if ( client.IsValid() )
		{
			client.VoiceLevel = level;
			client.TimeSinceLastVoice = 0;
		}

		UI.VoiceChatDisplay.Instance?.OnVoicePlayed( client, level );
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ForceStateChange( new WaitingState() );
	}

	[Event.Tick]
	private void Tick()
	{
		State?.OnTick();
	}

	private void OnStateChanged( BaseState oldState, BaseState newState )
	{
		_lastState?.Finish();
		_lastState = newState;
		_lastState.Start();

		Event.Run( TTTEvent.Game.StateChanged, oldState, newState );
	}
}
