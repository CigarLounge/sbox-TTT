using Sandbox;

namespace TTT;

[Hammer.Skip]
public partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net, Change]
	public BaseRound Round { get; private set; }
	private BaseRound _lastRound;

	[Net]
	public MapSelectionHandler MapSelection { get; set; } = new();

	public MapHandler MapHandler { get; private set; }

	public Game()
	{
		_ = MapSelection.Load();

		if ( IsServer )
		{
			new UI.Hud
			{
				Parent = this
			};
		}
	}

	public override void Simulate( Client cl )
	{
		Round.OnTick();

		base.Simulate( cl );
	}

	/// <summary>
	/// Changes the round if minimum players is met. Otherwise, force changes to "WaitingRound"
	/// </summary>
	/// <param name="round"> The round to change to if minimum players is met.</param>
	public void ChangeRound( BaseRound round )
	{
		Assert.NotNull( round );

		ForceRoundChange( Utils.HasMinimumPlayers() ? round : new WaitingRound() );
	}

	/// <summary>
	/// Force changes a round regardless of player count.
	/// </summary>
	/// <param name="round"> The round to change to.</param>
	public void ForceRoundChange( BaseRound round )
	{
		Host.AssertServer();

		Round?.Finish();
		BaseRound oldRound = Round;
		Round = round;
		Round.Start();

		Event.Run( TTTEvent.Game.RoundChanged, oldRound, Round );
	}

	public override void DoPlayerNoclip( Client client )
	{
		// Do nothing. The player can't noclip in this mode.
	}

	public override void ClientJoined( Client client )
	{
		Player player = new();
		client.Pawn = player;
		client.SetValue( RawStrings.Spectator, true );
		Round.OnPlayerJoin( client.Pawn as Player );

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has joined" );

		base.ClientJoined( client );
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
		Host.AssertServer();

		if ( source.Name.Equals( dest.Name ) || source.Pawn is not Player sourcePlayer || dest.Pawn is not Player destPlayer )
			return false;

		if ( Round is InProgressRound && !sourcePlayer.IsAlive() && destPlayer.IsAlive() )
			return false;

		return true;
	}

	/// <summary>
	/// Someone is speaking via voice chat. This might be someone in your game,
	/// or in your party, or in your lobby.
	/// </summary>
	public override void OnVoicePlayed( long playerId, float level )
	{
		Client client = null;

		foreach ( Client loopClient in Client.All )
		{
			if ( loopClient.PlayerId == playerId )
			{
				client = loopClient;
				break;
			}
		}

		if ( client == null || !client.IsValid() )
			return;

		UI.VoiceChatDisplay.Instance?.OnVoicePlayed( client, level );
	}

	public override void PostLevelLoaded()
	{
		base.PostLevelLoaded();

		ForceRoundChange( new WaitingRound() );
		MapHandler = new();
	}

	private void OnRoundChanged( BaseRound oldRound, BaseRound newRound )
	{
		_lastRound?.Finish();
		_lastRound = newRound;
		_lastRound.Start();

		Event.Run( TTTEvent.Game.RoundChanged, oldRound, newRound );
	}
}
