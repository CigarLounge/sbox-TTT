using Sandbox;
using System.Collections.Generic;

namespace TTT;

public partial class Game : Sandbox.Game
{
	public static new Game Current { get; private set; }

	[Net, Change]
	public BaseState State { get; private set; }
	private BaseState _lastState;

	[Net]
	public IList<string> MapVoteIdents { get; set; }

	[Net]
	public int TotalRoundsPlayed { get; set; }

	public int RTVCount { get; set; }

	public Game()
	{
		Current = this;

		if ( IsClient )
			_ = new UI.Hud();

		if ( Host.IsDedicatedServer )
			LoadBannedClients();

		LoadResources();
	}

	/// <summary>
	/// Changes the state if minimum players is met. Otherwise, force changes to "WaitingState"
	/// </summary>
	/// <param name="state"> The state to change to if minimum players is met.</param>
	public void ChangeState( BaseState state )
	{
		Host.AssertServer();
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
		State = state;
		State.Start();
	}

	public override void OnKilled( Entity pawn )
	{
		// Do nothing. Base implementation just adds to a kill feed and prints to console.
	}

	public override void ClientJoined( Client client )
	{
		Event.Run( GameEvent.Client.Joined, client );

		var player = new Player( client );

		State.OnPlayerJoin( player );

		UI.TextChat.AddInfo( To.Everyone, $"{client.Name} has joined" );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		Event.Run( GameEvent.Client.Disconnected, client );
		State.OnPlayerLeave( client.Pawn as Player );

		UI.TextChat.AddInfo( To.Everyone, $"{client.Name} has left ({reason})" );

		// Only delete the pawn if they are alive.
		// Keep the dead body otherwise on disconnect.
		if ( client.Pawn.IsValid() && client.Pawn.IsAlive() )
			client.Pawn.Delete();

		client.Pawn = null;
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		if ( source.Pawn is not Player sourcePlayer || dest.Pawn is not Player destPlayer )
			return false;

		if ( destPlayer.MuteFilter == MuteFilter.All )
			return false;

		if ( !sourcePlayer.IsAlive() && !destPlayer.CanHearSpectators )
			return false;

		if ( sourcePlayer.IsAlive() && !destPlayer.CanHearAlivePlayers )
			return false;

		return true;
	}

	public override void OnVoicePlayed( Client client )
	{
		UI.VoiceChatDisplay.Instance?.OnVoicePlayed( client );
	}

	public override void PostLevelLoaded()
	{
		ForceStateChange( new WaitingState() );
	}

	public override void Shutdown()
	{
		if ( Host.IsDedicatedServer )
			FileSystem.Data.WriteJson( BanFilePath, BannedClients );
	}

	[Event.Tick]
	private void Tick()
	{
		State?.OnTick();
	}

	private static void LoadResources()
	{
		Detective.Hat = ResourceLibrary.Get<Clothing>( "models/detective_hat/detective_hat.clothing" );
	}

	private void OnStateChanged( BaseState oldState, BaseState newState )
	{
		_lastState?.Finish();
		_lastState = newState;
		_lastState?.Start();
	}
}
