using Sandbox;

namespace TTT;

public abstract partial class BaseCamera : BaseNetworkable
{
	public virtual void FrameSimulate( Player player ) { }
	public virtual void Simulate( Player player ) { }
}
