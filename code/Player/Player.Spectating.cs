using Sandbox;

namespace TTT;

public partial class Player
{
	[Net, Local]
	public bool IsForcedSpectator { get; set; } = false;

	private Player _spectatedPlayer;
	public Player CurrentPlayer
	{
		get => _spectatedPlayer ?? this;
		set
		{
			_spectatedPlayer = value == this ? null : value;
		}
	}

	public bool IsSpectatingPlayer => _spectatedPlayer.IsValid();
	public bool IsSpectator => Client.GetValue<bool>( RawStrings.Spectator );

	private int _targetIdx = 0;

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsClient )
			return;

		var localPlayer = Local.Pawn as Player;
		if ( localPlayer.IsSpectatingPlayer && localPlayer.CurrentPlayer == player )
			localPlayer.CameraMode = new FreeSpectateCamera();
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
			CameraMode = new FreeSpectateCamera();
			return;
		}

		if ( CameraMode is FreeSpectateCamera )
			CameraMode = new ThirdPersonSpectateCamera();
		else if ( CameraMode is ThirdPersonSpectateCamera )
			CameraMode = new FirstPersonSpectatorCamera();
	}

	public void UpdateSpectatedPlayer( bool cycleForward = true )
	{
		var oldSpectatedPlayer = CurrentPlayer;
		var players = Utils.GetAlivePlayers();

		if ( players.Count > 0 )
		{
			_targetIdx = cycleForward ? _targetIdx + 1 : _targetIdx - 1;

			if ( _targetIdx >= players.Count )
				_targetIdx = 0;
			else if ( _targetIdx < 0 )
				_targetIdx = players.Count - 1;

			CurrentPlayer = players[_targetIdx];
		}

		if ( CameraMode is ISpectateCamera camera )
			camera.OnUpdateSpectatedPlayer( oldSpectatedPlayer, CurrentPlayer );
	}

	public void MakeSpectator( bool useRagdollCamera = true )
	{
		EnableAllCollisions = false;
		EnableDrawing = false;
		Controller = null;
		CameraMode = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();
		LifeState = LifeState.Dead;
		Health = 0f;

		DelayedDeathCameraChange();
	}

	private async void DelayedDeathCameraChange()
	{
		// If the player is still watching their ragdoll, automatically
		// move them to a free spectate camera.
		await Task.DelaySeconds( 2 );

		if ( CameraMode is RagdollSpectateCamera )
			CameraMode = new FreeSpectateCamera();
	}

	public void ToggleForcedSpectator()
	{
		IsForcedSpectator = !IsForcedSpectator;

		if ( Game.Current.Round is PreRound or WaitingRound )
			Client.SetValue( RawStrings.Spectator, IsForcedSpectator );

		if ( !IsForcedSpectator || !this.IsAlive() )
			return;

		TakeDamage( DamageInfo.Generic( 1000 ) );
	}
}
