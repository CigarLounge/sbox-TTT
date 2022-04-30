using Sandbox;
using System.Linq;

namespace TTT;

public partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net, Change]
	public BaseState Round { get; private set; }
	private BaseState _lastRound;

	[Net]
	public int TotalRoundsPlayed { get; set; }

	public int RTVCount { get; set; }

	public MapHandler MapHandler { get; private set; }

	public Game()
	{
		if ( IsServer )
			_ = new UI.Hud();
	}

	/// <summary>
	/// Changes the round if minimum players is met. Otherwise, force changes to "WaitingRound"
	/// </summary>
	/// <param name="round"> The round to change to if minimum players is met.</param>
	public void ChangeRound( BaseState round )
	{
		Assert.NotNull( round );

		ForceRoundChange( Utils.HasMinimumPlayers() ? round : new WaitingState() );
	}

	/// <summary>
	/// Force changes a round regardless of player count.
	/// </summary>
	/// <param name="round"> The round to change to.</param>
	public void ForceRoundChange( BaseState round )
	{
		Host.AssertServer();

		Round?.Finish();
		var oldRound = Round;
		Round = round;
		Round.Start();

		Event.Run( TTTEvent.Game.StateChanged, oldRound, Round );
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

		Round.OnPlayerJoin( player );

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has joined" );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		Round.OnPlayerLeave( client.Pawn as Player );

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

		if ( Round is InProgress && !source.Pawn.IsAlive() && dest.Pawn.IsAlive() )
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

		ForceRoundChange( new WaitingState() );
		MapHandler = new();
	}

	[Event.Tick]
	private void Tick()
	{
		Round?.OnTick();
	}

	private void OnRoundChanged( BaseState oldRound, BaseState newRound )
	{
		_lastRound?.Finish();
		_lastRound = newRound;
		_lastRound.Start();

		Event.Run( TTTEvent.Game.StateChanged, oldRound, newRound );
	}
}
