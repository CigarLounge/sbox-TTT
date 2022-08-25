using Sandbox;

namespace TTT;


public partial class Player
{
	[Net, Local]
	public bool IsForcedSpectator { get; private set; } = false;

	private Player _spectatedPlayer;
	public Player CurrentPlayer
	{
		get => _spectatedPlayer ?? this;
		set
		{
			_spectatedPlayer = value == this ? null : value;
		}
	}
	public bool IsSpectator => Status == PlayerStatus.Spectator;
	public bool IsSpectatingPlayer => _spectatedPlayer.IsValid();
	private int _spectatorIndex = 0;

	public void ToggleForcedSpectator()
	{
		IsForcedSpectator = !IsForcedSpectator;

		if ( !IsForcedSpectator || !this.IsAlive() )
			return;

		this.Kill();
	}

	public void UpdateSpectatedPlayer( int increment = 0 )
	{
		var players = Utils.GetAlivePlayers();
		if ( players.Count > 0 )
		{
			_spectatorIndex += increment;

			if ( _spectatorIndex >= players.Count )
				_spectatorIndex = 0;
			else if ( _spectatorIndex < 0 )
				_spectatorIndex = players.Count - 1;

			CurrentPlayer = players[_spectatorIndex];
		}

		if ( Camera is ISpectateCamera camera )
			camera.OnUpdateSpectatedPlayer( CurrentPlayer );
	}

	public void MakeSpectator( bool useRagdollCamera = true )
	{
		Client.VoiceStereo = true;
		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableTouch = false;
		Health = 0f;
		LifeState = LifeState.Dead;

		if ( Camera is not ISpectateCamera )
			Camera = useRagdollCamera ? new FollowEntityCamera( Corpse ) : new FreeSpectateCamera();

		DelayedDeathCameraChange();
	}

	private async void DelayedDeathCameraChange()
	{
		// If the player is still watching their ragdoll, automatically
		// move them to a free spectate camera.
		await GameTask.DelaySeconds( 2 );

		if ( Camera is FollowEntityCamera followCamera && followCamera.FollowedEntity is Corpse )
			Camera = new FreeSpectateCamera();
	}

	private void ChangeSpectateCamera()
	{
		if ( !Input.Pressed( InputButton.Jump ) )
			return;

		Camera = Camera switch
		{
			FreeSpectateCamera => new ThirdPersonCamera(),
			ThirdPersonCamera => new FirstPersonSpectatorCamera(),
			FollowEntityCamera or FirstPersonSpectatorCamera or _ => new FreeSpectateCamera(),
		};
	}

	[ConCmd.Server]
	public static void SpectatePlayer( int spectatedPlayerIndent )
	{
		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		if ( player.IsAlive() )
			return;

		var target = FindByIndex( spectatedPlayerIndent );
		if ( target is not Player spectatedPlayer )
			return;

		player.Camera = new ThirdPersonSpectateCamera { InitialSpectatedPlayer = spectatedPlayer };
	}

	[GameEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsClient )
			return;

		var localPlayer = Local.Pawn as Player;

		if ( localPlayer.IsSpectatingPlayer && localPlayer.CurrentPlayer == player )
			localPlayer.Camera = new FreeSpectateCamera();
	}
}
