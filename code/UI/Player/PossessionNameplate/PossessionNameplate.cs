using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionNameplate : WorldPanel
{
	private Label PlayerName { get; init; }
	private readonly Prop _prop;

	public PossessionNameplate( Prop prop )
	{
		PlayerName.SetText( prop.Owner.Client.Name );
		SceneObject.Flags.ViewModelLayer = true;
		_prop = prop;
	}

	[Event.Client.Frame]
	private void FrameUpdate()
	{
		var tx = Transform;
		tx.Position = _prop.WorldSpaceBounds.Center + (Vector3.Up * _prop.Model.RenderBounds.Maxs);
		tx.Rotation = Camera.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		Transform = tx;
	}
}
