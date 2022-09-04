using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SpectatingHint : Panel
{
	private Panel FindTargetHint { get; init; }
	private Panel SwapSpectatingPlayerPanel { get; init; }
	private Label PlayerName { get; init; }
	private Label SpectatingHintLabel { get; init; }
	private InputGlyph SpectatingGlyph { get; init; }

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

		SwapSpectatingPlayerPanel.EnableFade( player.IsSpectatingPlayer );
		if ( player.IsSpectatingPlayer )
		{
			PlayerName.Text = player.CurrentPlayer?.Client?.Name;
			PlayerName.Style.FontColor = player.CurrentPlayer?.Role?.Color;
		}
	}
}
