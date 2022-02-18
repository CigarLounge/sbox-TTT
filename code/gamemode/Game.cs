using System;

using Sandbox;

using TTT.Events;
using TTT.Map;
using TTT.Player;
using TTT.Rounds;

namespace TTT.Gamemode;

public partial class Game : Sandbox.Game
{
	public new static Game Current => Sandbox.Game.Current as Game;

	[Net, Change]
	public BaseRound Round { get; private set; } = new TTT.Rounds.WaitingRound();

	[Net]
	public MapSelectionHandler MapSelection { get; set; } = new();

	public MapHandler MapHandler { get; private set; }

	// [ConVar.Replicated("ttt_debug")]
	public bool Debug { get; set; } = true;

	public Game()
	{
		_ = MapSelection.Load();

		if ( IsServer )
		{
			_ = new UI.Hud
			{
				Parent = this
			};
		}
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

		Round.Finish();
		BaseRound oldRound = Round;
		Round = round;
		Round.Start();
	}

	public override void DoPlayerNoclip( Client client )
	{
		// Do nothing. The player can't noclip in this mode.
	}

	public override void DoPlayerSuicide( Client client )
	{
		if ( client.Pawn is TTTPlayer player && player.LifeState == LifeState.Alive )
		{
			base.DoPlayerSuicide( client );
		}
	}

	public override void OnKilled( Entity entity )
	{
		if ( entity is TTTPlayer player )
		{
			Round.OnPlayerKilled( player );
		}

		base.OnKilled( entity );
	}

	public override void ClientJoined( Client client )
	{
		Round.OnPlayerJoin( client.Pawn as TTTPlayer );
		TTTPlayer player = new();
		client.Pawn = player;

		base.ClientJoined( client );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		Log.Info( client.Name + " left, checking minimum player count..." );

		Round.OnPlayerLeave( client.Pawn as TTTPlayer );

		Log.Info( $"\"{client.Name}\" has left the game ({reason})" );
		UI.ChatBox.AddInformation( To.Everyone, $"{client.Name} has left ({reason})", $"avatar:{client.PlayerId}" );

		// Only delete the pawn if they are alive.
		// Keep the dead body otherwise on disconnect.
		if ( client.Pawn.IsValid() && client.Pawn.LifeState == LifeState.Alive )
		{
			client.Pawn.Delete();
			client.Pawn = null;
		}
	}

	public override bool CanHearPlayerVoice( Client source, Client dest )
	{
		Host.AssertServer();

		if ( source.Name.Equals( dest.Name ) || source.Pawn is not TTTPlayer sourcePlayer || dest.Pawn is not TTTPlayer destPlayer )
		{
			return false;
		}

		if ( Round is InProgressRound && sourcePlayer.LifeState == LifeState.Dead && destPlayer.LifeState == LifeState.Alive )
		{
			return false;
		}

		if ( sourcePlayer.IsTeamVoiceChatEnabled && destPlayer.Team != sourcePlayer.Team )
		{
			return false;
		}

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
		{
			return;
		}

		if ( client.Pawn is TTTPlayer player )
		{
			player.IsSpeaking = true;
		}

		UI.VoiceChatDisplay.Instance?.OnVoicePlayed( client, level );
	}

	public override void PostLevelLoaded()
	{
		StartGameTimer();

		base.PostLevelLoaded();

		MapHandler = new();
	}

	private async void StartGameTimer()
	{
		ForceRoundChange( new WaitingRound() );

		while ( true )
		{
			try
			{
				OnGameSecond();

				await GameTask.DelaySeconds( 1 );
			}
			catch ( Exception e )
			{
				if ( e.Message.Trim() == "A task was canceled." )
				{
					return;
				}

				Log.Error( $"[TASK] {e.Message}: {e.StackTrace}" );
			}
		}
	}

	private void OnGameSecond()
	{
		Round?.OnSecond();
	}

	public void OnRoundChanged( BaseRound oldRound, BaseRound newRound )
	{
		Event.Run( TTTEvent.Game.RoundChanged, oldRound, newRound );
	}
}
