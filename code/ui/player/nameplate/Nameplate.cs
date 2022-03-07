using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Nameplate : EntityHintPanel
{
	public Player Player;

	private readonly Panel _labelHolder;
	private readonly Label _nameLabel;
	private readonly Label _roleLabel;
	private readonly Label _damageIndicatorLabel;

	public Nameplate( Player player ) : base()
	{
		Player = player;

		StyleSheet.Load( "/ui/player/nameplate/Nameplate.scss" );

		_labelHolder = Add.Panel( "label-holder" );

		_nameLabel = _labelHolder.Add.Label( "", "name" );
		_nameLabel.AddClass( "text-shadow" );
		_damageIndicatorLabel = _labelHolder.Add.Label( "", "damage-indicator" );
		_damageIndicatorLabel.AddClass( "text-shadow" );
		_roleLabel = _labelHolder.Add.Label( "", "role" );
		_roleLabel.AddClass( "text-shadow-light" );

		this.Enabled( false );
	}

	public override void UpdateHintPanel( string text, string subtext = "" )
	{
		SetClass( "disabled", !this.IsEnabled() );

		if ( !this.IsEnabled() || !Player.IsValid() )
			return;

		// Network sync workaround
		if ( Player.Health == 0 && Player.IsAlive() )
		{
			_damageIndicatorLabel.Text = "";
		}
		else
		{
			var health = Player.Health / Player.MaxHealth * 100;
			var healthGroup = Player.GetHealthGroup( health );

			_damageIndicatorLabel.Style.FontColor = healthGroup.Color;
			_damageIndicatorLabel.Text = healthGroup.Title;
		}

		_nameLabel.Text = Player.Client?.Name ?? "";
		if ( Player.Role is not NoneRole && Player.Role is not InnocentRole )
		{
			_roleLabel.Text = Player.Role.Info.Title;
			_roleLabel.Style.FontColor = Player.Role.Info.Color;
		}
	}
}
