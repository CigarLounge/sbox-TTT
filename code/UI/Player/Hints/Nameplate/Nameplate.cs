using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Nameplate : EntityHintPanel
{
	public readonly Player _player;

	private readonly Panel _labelHolder;
	private readonly Label _nameLabel;
	private readonly Label _roleLabel;
	private readonly Label _damageIndicatorLabel;

	public Nameplate( Player player )
	{
		_player = player;

		StyleSheet.Load( "/ui/player/hints/nameplate/Nameplate.scss" );

		_labelHolder = Add.Panel( "label-holder" );

		_nameLabel = _labelHolder.Add.Label( "", "name" );
		_nameLabel.AddClass( "text-shadow" );
		_damageIndicatorLabel = _labelHolder.Add.Label( "", "damage-indicator" );
		_damageIndicatorLabel.AddClass( "text-shadow" );
		_roleLabel = _labelHolder.Add.Label( "", "role" );
		_roleLabel.AddClass( "text-shadow-light" );

		this.Enabled( false );
	}

	public override void Tick()
	{
		SetClass( "disabled", !this.IsEnabled() );

		if ( !this.IsEnabled() || !_player.IsValid() )
			return;

		// Network sync workaround
		if ( _player.Health == 0 && _player.IsAlive() )
		{
			_damageIndicatorLabel.Text = "";
		}
		else
		{
			var health = _player.Health / _player.MaxHealth * 100;
			var healthGroup = _player.GetHealthGroup( health );

			_damageIndicatorLabel.Style.FontColor = healthGroup.Color;
			_damageIndicatorLabel.Text = healthGroup.Title;
		}

		_nameLabel.Text = _player.Client?.Name ?? "";
		if ( _player.Role is not NoneRole && _player.Role is not InnocentRole )
		{
			_roleLabel.Text = _player.Role.Title;
			_roleLabel.Style.FontColor = _player.Role.Color;
		}
	}
}
