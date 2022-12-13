using Sandbox;

namespace TTT;


public partial class Player
{
	[Net, Local]
	public BaseCamera CurrentCamera { get; set; }

	[Net, Local]
	public bool IsForcedSpectator { get; private set; } = false;

	public bool IsSpectator => Status == PlayerStatus.Spectator;

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
		CurrentCamera = useRagdollCamera ? new FollowEntityCamera( Corpse ) : new FreeCamera();
	}
}
