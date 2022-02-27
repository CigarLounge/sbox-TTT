using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class PlayerInfoDisplay : Panel
{
	private Panel _healthPanel;
	private Label _healthLabel;
	private Panel _creditPanel;
	private Label _creditLabel;

	public PlayerInfoDisplay() : base()
	{
		StyleSheet.Load( "/ui/player/playerinfo/PlayerInfoDisplay.scss" );

		AddClass( "background-color-primary" );
		AddClass( "rounded" );
		AddClass( "opacity-heavy" );
		AddClass( "text-shadow" );

		_healthPanel = new Panel( this );
		_creditPanel = new Panel( this );

		_healthLabel = _healthPanel.Add.Label();
		_healthLabel.AddClass( "info" );

		_creditLabel = _creditPanel.Add.Label();
		_creditLabel.AddClass( "info" );
	}

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		this.Enabled( player.IsAlive() || player.IsSpectatingPlayer );
		if ( !this.IsEnabled() )
			return;

		_healthLabel.Text = $"✚ {player.CurrentPlayer.Health.CeilToInt()}";
		_creditLabel.Text = $"$ {player.CurrentPlayer.Credits}";
	}
}
