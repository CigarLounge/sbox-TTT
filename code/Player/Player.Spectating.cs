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
	private int _targetSpectatorIndex = 0;

	public void ToggleForcedSpectator()
	{
		IsForcedSpectator = !IsForcedSpectator;

		if ( !IsForcedSpectator || !this.IsAlive() )
			return;

		this.Kill();
	}

	public void UpdateSpectatedPlayer( int increment = 0 )
	{
		var oldSpectatedPlayer = CurrentPlayer;
		var players = Utils.GetAlivePlayers();

		if ( players.Count > 0 )
		{
			_targetSpectatorIndex += increment;

			if ( _targetSpectatorIndex >= players.Count )
				_targetSpectatorIndex = 0;
			else if ( _targetSpectatorIndex < 0 )
				_targetSpectatorIndex = players.Count - 1;

			CurrentPlayer = players[_targetSpectatorIndex];
		}

		if ( Camera is ISpectateCamera camera )
			camera.OnUpdateSpectatedPlayer( oldSpectatedPlayer, CurrentPlayer );
	}

	public void MakeSpectator( bool useRagdollCamera = true )
	{
		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		Health = 0f;
		LifeState = LifeState.Dead;

		if ( Camera is not ISpectateCamera )
			Camera = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();

		DelayedDeathCameraChange();
	}

	private async void DelayedDeathCameraChange()
	{
		// If the player is still watching their ragdoll, automatically
		// move them to a free spectate camera.
		await GameTask.DelaySeconds( 2 );

		if ( Camera is RagdollSpectateCamera )
			Camera = new FreeSpectateCamera();
	}

	private void ChangeSpectateCamera()
	{
		if ( !Input.Pressed( InputButton.Jump ) )
			return;

		if ( Camera is RagdollSpectateCamera || Camera is FirstPersonSpectatorCamera )
		{
			Camera = new FreeSpectateCamera();
			return;
		}

		var spectatablePlayers = Utils.GetAlivePlayers().Count > 0;
		if ( !spectatablePlayers )
		{
			if ( Camera is not FreeSpectateCamera )
				Camera = new FreeSpectateCamera();
			return;
		}

		if ( Camera is FreeSpectateCamera )
			Camera = new ThirdPersonSpectateCamera();
		else if ( Camera is ThirdPersonSpectateCamera )
			Camera = new FirstPersonSpectatorCamera();
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsClient )
			return;

		var localPlayer = Local.Pawn as Player;

		if ( localPlayer.IsSpectatingPlayer && localPlayer.CurrentPlayer == player )
			localPlayer.Camera = new FreeSpectateCamera();
	}
}
