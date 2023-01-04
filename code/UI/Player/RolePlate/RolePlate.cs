using Sandbox;

namespace TTT.UI;

public partial class RolePlate : EntityComponent<Player>
{
	private RolePlateWorldPanel _roleWorldPanel;

	protected override void OnActivate()
	{
		base.OnActivate();

		_roleWorldPanel = new RolePlateWorldPanel() { Icon = Entity.Role.Info.IconPath };
		_roleWorldPanel.SceneObject.Flags.ViewModelLayer = true;
	}

	protected override void OnDeactivate()
	{
		base.OnDeactivate();

		_roleWorldPanel?.Delete();
		_roleWorldPanel = null;
	}

	[Event.Client.Frame]
	private void FrameUpdate()
	{
		_roleWorldPanel.Enabled( !Entity.IsLocalPawn && Entity.IsAlive );

		if ( !_roleWorldPanel.IsEnabled() )
			return;

		var tx = Entity.GetBoneTransform( "head" );
		tx.Position += Vector3.Up * 20f;
		tx.Rotation = Camera.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		_roleWorldPanel.Transform = tx;
	}
}
