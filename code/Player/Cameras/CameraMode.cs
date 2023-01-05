using Sandbox;

namespace TTT;

public abstract class CameraMode
{
	public static CameraMode Current { get; internal set; } = new FreeCamera();

	/// <summary>
	/// Any camera inputs that need to happen every frame.
	/// </summary>
	public virtual void BuildInput() { }

	/// <summary>
	/// Update the camera position here since it happens every frame.
	/// </summary>
	public virtual void FrameSimulate( IClient client ) { }
}
