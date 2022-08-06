using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionNameplate : WorldPanel
{
	private Label PlayerName { get; set; }
	private readonly Player _player;
	private readonly Prop _prop;

	public PossessionNameplate( Player player, Prop prop )
	{
		_player = player;
		_prop = prop;
		PlayerName.SetText( player.SteamName );
		SceneObject.Flags.ViewModelLayer = true;
	}

	[Event.Frame]
	private void FrameUpdate()
	{
		if ( !_player.IsValid() || !_prop.IsValid() )
		{
			Delete();
			return;
		}

		var tx = Transform;
		tx.Position = _prop.WorldSpaceBounds.Center + (Vector3.Up * _prop.Model.RenderBounds.Maxs);
		tx.Rotation = CurrentView.Rotation.RotateAroundAxis( Vector3.Up, 180f );

		Transform = tx;
	}
}
