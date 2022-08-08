using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SpectatingHint : Panel
{
	private Panel SwapPanel { get; set; }
	private Label PlayerName { get; set; }
	private Label HintAction { get; init; }
	private InputGlyph HintGlyph { get; init; }

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( !player.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		if ( player.Prop.IsValid() )
		{
			HintGlyph.SetButton( InputButton.Duck );
			HintAction.SetText( "to no longer possess the prop" );
		}
		else
		{
			HintGlyph.SetButton( InputButton.Jump );
			HintAction.SetText( "to change spectating camera mode" );
		}

		SwapPanel.EnableFade( player.IsSpectatingPlayer );
		if ( SwapPanel.IsEnabled() )
		{
			PlayerName.Text = player.CurrentPlayer?.Client?.Name;
			PlayerName.Style.FontColor = player.CurrentPlayer?.Role?.Color;
		}
	}
}
