using Sandbox;
using Sandbox.Diagnostics;
using System.Collections.Generic;

namespace TTT;

public partial class GameManager : Sandbox.GameManager
{
	public static new GameManager Current { get; private set; }

	[Net, Change]
	public BaseState State { get; private set; }
	private BaseState _lastState;

	[Net]
	public IList<string> MapVoteIdents { get; set; }

	[Net]
	public int TotalRoundsPlayed { get; set; }

	public int RTVCount { get; set; }

	[Net]
	public RealTimeUntil MapTimer { get; set; }

	public GameManager()
	{
		Current = this;

		if ( Game.IsClient )
			_ = new UI.Hud();

		if ( Game.IsDedicatedServer )
			LoadBannedClients();

		LoadResources();
	}

	public override void FrameSimulate( IClient client )
	{
		if ( client.Pawn is not Entity entity || !entity.IsValid() || !entity.IsAuthority )
			return;

		entity.FrameSimulate( client );
		CameraMode.Current.FrameSimulate( client );
	}

	public override void BuildInput()
	{
		CameraMode.Current.BuildInput();

		base.BuildInput();
	}

	/// <summary>
	/// Changes the state if minimum players is met. Otherwise, force changes to "WaitingState"
	/// </summary>
	/// <param name="state"> The state to change to if minimum players is met.</param>
	public void ChangeState( BaseState state )
	{
		Game.AssertServer();
		Assert.NotNull( state );

		var HasMinimumPlayers = Utils.GetPlayersWhere( p => !p.IsForcedSpectator ).Count >= MinPlayers;
		ForceStateChange( HasMinimumPlayers ? state : new WaitingState() );
	}

	/// <summary>
	/// Force changes a state regardless of player count.
	/// </summary>
	/// <param name="state"> The state to change to.</param>
	public void ForceStateChange( BaseState state )
	{
		Game.AssertServer();

		State?.Finish();
		State = state;
		State.Start();
	}

	public override void OnKilled( Entity pawn )
	{
		// Do nothing. Base implementation just adds to a kill feed and prints to console.
	}

	public override void ClientJoined( IClient client )
	{
		Event.Run( GameEvent.Client.Joined, client );

		var player = new Player( client );

		State.OnPlayerJoin( player );

		UI.TextChat.AddInfo( To.Everyone, $"{client.Name} has joined" );
	}

	public override void ClientDisconnect( IClient client, NetworkDisconnectionReason reason )
	{
		Event.Run( GameEvent.Client.Disconnected, client );
		State.OnPlayerLeave( client.Pawn as Player );

		UI.TextChat.AddInfo( To.Everyone, $"{client.Name} has left ({reason})" );

		// Only delete the pawn if they are alive.
		// Keep the dead body otherwise on disconnect.
		var player = client.Pawn as Player;
		if ( player.IsValid() && player.IsAlive )
			client.Pawn.Delete();

		client.Pawn = null;
	}

	public override bool CanHearPlayerVoice( IClient source, IClient dest )
	{
		if ( source.Pawn is not Player sourcePlayer || dest.Pawn is not Player destPlayer )
			return false;

		if ( destPlayer.MuteFilter == MuteFilter.All )
			return false;

		if ( !sourcePlayer.IsAlive && !destPlayer.CanHearSpectators )
			return false;

		if ( sourcePlayer.IsAlive && !destPlayer.CanHearAlivePlayers )
			return false;

		return true;
	}

	public override void OnVoicePlayed( IClient client )
	{
		UI.VoiceChatDisplay.Instance?.OnVoicePlayed( client );
	}

	public override void PostLevelLoaded()
	{
		MapTimer = 60 * TimeLimitMinutes;
		ForceStateChange( new WaitingState() );
	}

	public override void Shutdown()
	{
		if ( Game.IsDedicatedServer )
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
