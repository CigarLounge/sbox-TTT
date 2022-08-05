using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class SpectatingHint : Panel
{
	private Panel SwapPanel { get; set; }
	private Label PlayerName { get; set; }
	private Label JumpAction { get; init; }
	private InputGlyph JumpGlyph { get; init; }

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( player.Components.TryGet<PropPossession>( out _ ) )
		{
			JumpGlyph.SetButton( InputButton.Duck );
			JumpAction.SetText( "to no longer possess the prop" );
		}
		else
		{
			JumpGlyph.SetButton( InputButton.Jump );
			JumpAction.SetText( "to change spectating camera mode" );
		}

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
