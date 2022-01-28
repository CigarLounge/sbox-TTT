using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTT.Player;

namespace TTT.UI
{
	public class PlayerRoleDisplay : Panel
	{
		private Label _roleLabel;

		public PlayerRoleDisplay() : base()
		{
			StyleSheet.Load( "/ui/generalhud/playerinfo/PlayerRoleDisplay.scss" );

			AddClass( "rounded" );
			AddClass( "opacity-heavy" );
			AddClass( "text-shadow" );

			_roleLabel = Add.Label();
			_roleLabel.AddClass( "centered" );
			_roleLabel.AddClass( "role-label" );

			OnRoleUpdate( Local.Pawn as TTTPlayer );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Local.Pawn is not TTTPlayer player )
			{
				return;
			}

			this.Enabled( !player.IsSpectator && !player.IsSpectatingPlayer && Gamemode.Game.Instance.Round is Rounds.InProgressRound );
		}

		[Event( Events.TTTEvent.Player.Role.Select )]
		private void OnRoleUpdate( TTTPlayer player )
		{
			if ( player == null || player != Local.Pawn as TTTPlayer )
			{
				return;
			}

			_roleLabel.Text = player.Role.Name;
			Style.BackgroundColor = player.Role.Color;
		}
	}
}
