using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SpectatingHint : Panel
{
	private Panel SwapPanel { get; set; }
	private Label PlayerName { get; set; }

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( !player.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		SwapPanel.EnableFade( player.IsSpectatingPlayer );
		if ( SwapPanel.IsEnabled() )
		{
			PlayerName.Text = player.CurrentPlayer?.Client?.Name;
			PlayerName.Style.FontColor = player.CurrentPlayer?.Role?.Color;
		}
	}
}
