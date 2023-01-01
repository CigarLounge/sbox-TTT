using Sandbox;

namespace TTT;


public partial class Player
{
	public bool IsForcedSpectator => Client.GetClientData<bool>( "forced_spectator" );
	public bool IsSpectator => Status == PlayerStatus.Spectator;

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
