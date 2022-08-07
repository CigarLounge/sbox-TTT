using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PossessionNameplate : WorldPanel
{
	public Player Player { get; init; }
	private Label PlayerName { get; set; }
	private readonly Prop _prop;

	public PossessionNameplate( Player player, Prop prop )
	{
		Player = player;
		PlayerName.SetText( player.SteamName );
		SceneObject.Flags.ViewModelLayer = true;
		_prop = prop;
	}

	[Event.Frame]
	private void FrameUpdate()
	{
		if ( !Player.IsValid() || !_prop.IsValid() )
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
