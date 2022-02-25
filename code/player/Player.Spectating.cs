using System.Collections.Generic;

using Sandbox;

namespace TTT;

public partial class Player
{
	private Player _spectatingPlayer;
	public Player CurrentPlayer
	{
		get => _spectatingPlayer ?? this;
		set
		{
			_spectatingPlayer = value == this ? null : value;
		}
	}

	public bool IsSpectatingPlayer => _spectatingPlayer.IsValid();
	public bool IsSpectator => CameraMode is IObservationCamera;


	private int _targetIdx = 0;

	[TTTEvent.Player.Died]
	private static void OnPlayerDied( Player deadPlayer )
	{
		if ( !Host.IsClient || Local.Pawn is not Player player )
			return;

		if ( player.IsSpectatingPlayer && player.CurrentPlayer == deadPlayer )
			player.UpdateObservatedPlayer();
	}

	private void ChangeSpectateCamera()
	{
		if ( Input.Pressed( InputButton.Jump ) )
		{
			CameraMode = CameraMode switch
			{
				RagdollSpectateCamera => new FreeSpectateCamera(),
				FreeSpectateCamera => new ThirdPersonSpectateCamera(),
				ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
				FirstPersonSpectatorCamera => new FreeSpectateCamera(),
				_ => CameraMode
			};
		}
	}

	public void UpdateObservatedPlayer()
	{
		Player oldObservatedPlayer = CurrentPlayer;

		CurrentPlayer = null;

		List<Player> players = Utils.GetAlivePlayers();

		if ( players.Count > 0 )
		{
			if ( ++_targetIdx >= players.Count )
			{
				_targetIdx = 0;
			}

			CurrentPlayer = players[_targetIdx];
		}

		if ( CameraMode is IObservationCamera camera )
		{
			camera.OnUpdateObservatedPlayer( oldObservatedPlayer, CurrentPlayer );
		}
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

		if ( IsForcedSpectator && LifeState == LifeState.Alive )
		{
			TakeDamage( DamageInfo.Generic( 1000 ) );

			if ( !Client.GetValue( RawStrings.ForcedSpectator, false ) )
			{
				Client.SetValue( RawStrings.ForcedSpectator, true );
			}
		}
	}
}
