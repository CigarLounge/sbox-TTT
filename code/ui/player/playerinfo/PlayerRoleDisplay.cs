using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class PlayerRoleDisplay : Panel
{
	private Label _roleLabel;

	public PlayerRoleDisplay() : base()
	{
		StyleSheet.Load( "/ui/player/playerinfo/PlayerRoleDisplay.scss" );

		AddClass( "rounded" );
		AddClass( "opacity-heavy" );
		AddClass( "text-shadow" );

		_roleLabel = Add.Label();
		_roleLabel.AddClass( "centered" );
		_roleLabel.AddClass( "role-label" );

		OnRoleUpdate( Local.Pawn as Player );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
		{
			return;
		}

		this.Enabled( !player.IsSpectator && !player.IsSpectatingPlayer && Game.Current.Round is InProgressRound );
	}

	[TTTEvent.Player.Role.Changed]
	private void OnRoleUpdate( Player player )
	{
		if ( player == null || player != Local.Pawn as Player )
		{
			return;
		}

		_roleLabel.Text = player.Role.Info.Name;
		Style.BackgroundColor = player.Role.Info.Color;
	}
}
