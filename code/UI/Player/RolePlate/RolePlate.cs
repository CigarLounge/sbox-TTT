using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class RolePlate : EntityComponent<Player>
{
	private WorldPanel _worldPanel;

	protected override void OnActivate()
	{
		base.OnActivate();

		_worldPanel = new WorldPanel();

		_worldPanel.StyleSheet.Load( "/UI/Player/RolePlate/RolePlate.scss" );
		_worldPanel.Add.Image( classname: "icon" ).Texture = Entity.Role.Info.Icon;
		_worldPanel.SceneObject.Flags.ViewModelLayer = true;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		_worldPanel?.Delete();
		_worldPanel = null;
	}

	/// <summary>
	/// Called for every plate, while it's active.
	/// </summary>
	[Event.Frame]
	private void FrameUpdate()
	{
		_worldPanel.SceneObject.RenderingEnabled = !Entity.IsFirstPersonMode && Entity.IsAlive();

		if ( !_worldPanel.SceneObject.RenderingEnabled )
			return;

		var tx = Entity.GetBoneTransform( "head" );
		tx.Position += Vector3.Up * 20.0f;
		tx.Rotation = CurrentView.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		_worldPanel.Transform = tx;
	}
}
