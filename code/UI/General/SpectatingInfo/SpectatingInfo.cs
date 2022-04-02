using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SpectatingInfo : Panel
{
	private Label PlayerName { get; set; }

	public override void Tick()
	{
		var player = Local.Pawn as Player;
		this.Enabled( player.IsSpectator );
		if ( !this.IsEnabled() )
			return;

		PlayerName.EnableFade( player.IsSpectatingPlayer );

		if ( PlayerName.IsEnabled() )
		{
			PlayerName.Text = player.CurrentPlayer?.Client?.Name;
			PlayerName.Style.FontColor = player.CurrentPlayer?.Role?.Color;
		}
	}
}
