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

	public bool IsSpectator
	{
		get => Client.GetValue( Strings.Spectator, true );
		set => Client.SetValue( Strings.Spectator, value );
	}

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

		if ( CameraMode is ISpectateCamera camera )
			camera.OnUpdateSpectatedPlayer( oldSpectatedPlayer, CurrentPlayer );
	}

	public void MakeSpectator( bool useRagdollCamera = true )
	{
		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		Health = 0f;
		LifeState = LifeState.Dead;

		if ( CameraMode is not ISpectateCamera )
			CameraMode = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();

		DelayedDeathCameraChange();
	}

	private async void DelayedDeathCameraChange()
	{
		// If the player is still watching their ragdoll, automatically
		// move them to a free spectate camera.
		await GameTask.DelaySeconds( 2 );

		if ( CameraMode is RagdollSpectateCamera )
			CameraMode = new FreeSpectateCamera();
	}

	private void ChangeSpectateCamera()
	{
		if ( !Input.Pressed( InputButton.Jump ) )
			return;

		if ( CameraMode is RagdollSpectateCamera || CameraMode is FirstPersonSpectatorCamera )
		{
			CameraMode = new FreeSpectateCamera();
			return;
		}

		var spectatablePlayers = Utils.GetAlivePlayers().Count > 0;
		if ( !spectatablePlayers )
		{
			if ( CameraMode is not FreeSpectateCamera )
				CameraMode = new FreeSpectateCamera();
			return;
		}

		if ( CameraMode is FreeSpectateCamera )
			CameraMode = new ThirdPersonSpectateCamera();
		else if ( CameraMode is ThirdPersonSpectateCamera )
			CameraMode = new FirstPersonSpectatorCamera();
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsClient )
			return;

		var localPlayer = Local.Pawn as Player;

		if ( localPlayer.IsSpectatingPlayer && localPlayer.CurrentPlayer == player )
			localPlayer.CameraMode = new FreeSpectateCamera();
	}
}
