using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class PossessionNameplate : WorldPanel
{
	private readonly Prop _prop;

	public PossessionNameplate( Prop prop )
	{
		_prop = prop;
		SceneObject.Flags.ViewModelLayer = true;
	}

	[GameEvent.Client.Frame]
	private void FrameUpdate()
	{
		var tx = Transform;
		tx.Position = _prop.WorldSpaceBounds.Center + (Vector3.Up * _prop.Model.RenderBounds.Maxs);
		tx.Rotation = Camera.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		Transform = tx;
	}
}
