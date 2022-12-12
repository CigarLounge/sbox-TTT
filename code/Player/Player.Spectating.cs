using Sandbox;

namespace TTT;


public partial class Player
{
	[Net, Predicted]
	public BaseCamera CurrentCamera { get; set; }

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

	public void ToggleForcedSpectator()
	{
		IsForcedSpectator = !IsForcedSpectator;

		if ( !IsForcedSpectator || !this.IsAlive() )
			return;

		this.Kill();
	}

	public void MakeSpectator( bool useRagdollCamera = true )
	{
		Client.Voice.WantsStereo = true;
		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableTouch = false;
		Health = 0f;
		LifeState = LifeState.Dead;
		CurrentCamera = useRagdollCamera ? new FollowEntityCamera( Corpse ) : new PlayerCamera(); ;
	}

	private void ChangeSpectateCamera()
	{
		// if ( !IsSpectatingPlayer && Camera is not FreeSpectateCamera and not FollowEntityCamera )
		// {
		// 	Camera = new FreeSpectateCamera();
		// 	return;
		// }

		// if ( !Input.Pressed( InputButton.Jump ) )
		// 	return;

		// Camera = Camera switch
		// {
		// 	FreeSpectateCamera => new ThirdPersonSpectateCamera(),
		// 	ThirdPersonSpectateCamera => new FirstPersonSpectatorCamera(),
		// 	FollowEntityCamera or FirstPersonSpectatorCamera or _ => new FreeSpectateCamera(),
		// };
	}

	[GameEvent.Player.Killed]
	private static async void OnPlayerKilled( Player player )
	{
		if ( Game.IsServer )
			return;

		player.CurrentCamera = new SpectatorCamera();

		// if ( player.IsLocalPawn )
		// {
		// 	// If the player is still watching their ragdoll, automatically
		// 	// move them to a free spectate camera after two seconds.
		// 	// await GameTask.DelaySeconds( 2 );

		// 	// if ( !player.IsAlive() && player.Camera is FollowEntityCamera followCamera && followCamera.FollowedEntity is Corpse )
		// 	// 	player.Camera = new FreeSpectateCamera();
		// }
		// else
		// {
		// 	var localPlayer = Game.LocalPawn as Player;
		// 	if ( localPlayer.IsSpectatingPlayer && localPlayer.CurrentPlayer == player )
		// 		localPlayer.Camera = new FreeSpectateCamera();
		// }
	}
}
