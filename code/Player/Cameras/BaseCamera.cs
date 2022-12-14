namespace TTT;

public abstract partial class BaseCamera
{
	/// <summary>
	/// Any camera inputs that need to happen every frame.
	/// </summary>
	public virtual void BuildInput( Player player ) { }

	/// <summary>
	/// Update the camera position here since it happens every frame.
	/// </summary>
	public virtual void FrameSimulate( Player player ) { }
}
