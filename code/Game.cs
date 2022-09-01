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

		LoadResources();
		LoadBannedClients();
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

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has joined" );
	}

	public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
	{
		Event.Run( GameEvent.Client.Disconnected, client );
		State.OnPlayerLeave( client.Pawn as Player );

		UI.ChatBox.AddInfo( To.Everyone, $"{client.Name} has left ({reason})" );

		// Only delete the pawn if they are alive.
		// Keep the dead body otherwise on disconnect.
		if ( client.Pawn.IsValid() && client.Pawn.IsAlive() )
			client.Pawn.Delete();

		client.Pawn = null;
	}

	public override void RenderHud()
	{
		if ( Local.Pawn is not Player player )
			return;

		var scale = Screen.Height / 1080.0f;
		var screenSize = Screen.Size / scale;
		var matrix = Matrix.CreateScale( scale );

		using ( Render.Draw2D.MatrixScope( matrix ) )
			player.RenderHud( screenSize );
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

		Player.ClothingPresets.Add( new List<Clothing>() );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/hat/balaclava/balaclava.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/hair/eyebrows/eyebrows_black.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/jacket/longsleeve/longsleeve.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/gloves/tactical_gloves/tactical_gloves.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/trousers/smarttrousers/trousers.smart.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.clothing" ) );
		Player.ClothingPresets[0].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/shoes/trainers/trainers.clothing" ) );

		Player.ClothingPresets.Add( new List<Clothing>() );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/hat/balaclava/balaclava.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/hair/eyebrows/eyebrows.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/shirt/flannel_shirt/variations/blue_shirt/blue_shirt.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/gloves/tactical_gloves/tactical_gloves.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/trousers/cargopants/cargo_pants_army.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/vest/tactical_vest/models/tactical_vest.clothing" ) );
		Player.ClothingPresets[1].Add( ResourceLibrary.Get<Clothing>( "models/citizen_clothes/shoes/trainers/trainers.clothing" ) );
	}

	private void OnStateChanged( BaseState oldState, BaseState newState )
	{
		_lastState?.Finish();
		_lastState = newState;
		_lastState?.Start();
	}
}
