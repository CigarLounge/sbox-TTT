using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class SpectatingHint : Panel
{
	private Panel FindTargetHint { get; set; }
	private Panel SwapSpectatingPlayerPanel { get; set; }
	private Label PlayerName { get; set; }
	private Label SpectatingHintLabel { get; set; }
	private InputGlyph SpectatingGlyph { get; set; }

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( !player.IsAlive() );
		if ( !this.IsEnabled() )
			return;

		var isPossessingProp = player.Prop.IsValid();

		FindTargetHint.Enabled( !isPossessingProp && !player.IsSpectatingPlayer );

		if ( isPossessingProp )
		{
			SpectatingGlyph.SetButton( InputButton.Duck );
			SpectatingHintLabel.SetText( "to no longer possess the prop" );
		}
		else
		{
			SpectatingGlyph.SetButton( InputButton.Jump );
			SpectatingHintLabel.SetText( "to change spectating camera mode" );
		}

		SwapSpectatingPlayerPanel.Enabled( player.IsSpectatingPlayer );
		if ( player.IsSpectatingPlayer )
		{
			PlayerName.Text = player.CurrentPlayer?.Client?.Name;
			PlayerName.Style.FontColor = player.CurrentPlayer?.Role?.Color;
		}
	}
}
