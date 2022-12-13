using Sandbox;

namespace TTT;

public abstract partial class BaseCamera : BaseNetworkable
{
	/// <summary>
	/// Any camera inputs that need to happen every frame.
	/// </summary>
	public virtual void BuildInput() { }

	/// <summary>
	/// Update the camera position here since it happens every frame.
	/// </summary>
	public virtual void FrameSimulate( Player player ) { }

	/// <summary>
	/// Can be used to swap between cameras.
	/// </summary>
	public virtual void Simulate( Player player ) { }
}
