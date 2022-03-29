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
			localPlayer.UpdateSpectatedPlayer();
	}

	private void ChangeSpectateCamera()
	{
		if ( !Input.Pressed( InputButton.Jump ) )
			return;

		CameraMode = CameraMode switch
		{
			RagdollSpectateCamera => new FreeSpectateCamera(),
			FreeSpectateCamera => new ThirdPersonSpectateCamera(),
			ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
			FirstPersonSpectatorCamera => new FreeSpectateCamera(),
			_ => CameraMode
		};
	}

	public void UpdateSpectatedPlayer()
	{
		var oldSpectatedPlayer = CurrentPlayer;
		var players = Utils.GetAlivePlayers();
		if ( players.Count > 0 )
		{
			if ( ++_targetIdx >= players.Count )
				_targetIdx = 0;

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

		if ( IsForcedSpectator && this.IsAlive() )
		{
			TakeDamage( DamageInfo.Generic( 1000 ) );
			Client.SetValue( RawStrings.Spectator, IsForcedSpectator );
		}
	}
}
