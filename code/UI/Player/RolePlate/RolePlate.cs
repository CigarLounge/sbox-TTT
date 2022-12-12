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

	[Event.Client.Frame]
	private void FrameUpdate()
	{
		_worldPanel.Enabled( !Entity.IsFirstPersonMode && Entity.IsAlive() );

		if ( !_worldPanel.IsEnabled() )
			return;

		var tx = Entity.GetBoneTransform( "head" );
		tx.Position += Vector3.Up * 20f;
		tx.Rotation = Sandbox.Camera.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		_worldPanel.Transform = tx;
	}
}
