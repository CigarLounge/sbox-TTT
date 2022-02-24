using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class TraitorTagComponent : EntityComponent<Player>
{
	TraitorTag TraitorTag;

	protected override void OnActivate()
	{
		TraitorTag = new TraitorTag();
	}

	protected override void OnDeactivate()
	{
		TraitorTag?.Delete();
		TraitorTag = null;
	}

	/// <summary>
	/// Called for every tag, while it's active
	/// </summary>
	[Event.Frame]
	private void FrameUpdate()
	{
		var tx = Entity.GetAttachment( "hat" ) ?? Entity.Transform;
		tx.Position += Vector3.Up * 5.0f;
		tx.Rotation = Rotation.LookAt( -CurrentView.Rotation.Forward );

		TraitorTag.Transform = tx;
	}

	[TTTEvent.Player.Role.Changed]
	private static void OnRoleChanged( Player player )
	{
		if ( !Host.IsClient || !player.IsAlive() || player.IsLocalPawn )
			return;

		if ( player.Team != Team.Traitors )
		{
			player.Components.RemoveAny<TraitorTagComponent>();
			return;
		}

		player.Components.GetOrCreate<TraitorTagComponent>();
	}

	[TTTEvent.Player.Died]
	private static void OnPlayerDied( Player player )
	{
		if ( !Host.IsClient || player.IsLocalPawn || player.Team != Team.Traitors )
			return;

		player.Components.RemoveAny<TraitorTagComponent>();
	}
}

public partial class TraitorTag : WorldPanel
{
	public TraitorTag()
	{
		StyleSheet.Load( "/ui/player/traitortag/TraitorTag.scss" );

		Add.Image( "ui/traitor-icon.png", "icon" );
	}
}
