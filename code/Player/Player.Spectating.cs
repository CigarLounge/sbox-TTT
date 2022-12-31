using Sandbox;

namespace TTT;


public partial class Player
{
	[Net, Local]
	public bool IsForcedSpectator { get; private set; } = false;

	public CameraMode CameraMode { get; set; }
	public bool IsSpectator => Status == PlayerStatus.Spectator;

	public void ToggleForcedSpectator()
	{
		IsForcedSpectator = !IsForcedSpectator;

		if ( !IsForcedSpectator || !this.IsAlive() )
			return;

		this.Kill();
	}

	public void MakeSpectator()
	{
		Client.Voice.WantsStereo = true;
		Controller = null;
		EnableAllCollisions = false;
		EnableDrawing = false;
		EnableTouch = false;
		Health = 0f;
		LifeState = LifeState.Dead;
	}
}
