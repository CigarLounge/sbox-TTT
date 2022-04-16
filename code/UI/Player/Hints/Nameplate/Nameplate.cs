using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class Nameplate : EntityHintPanel
{
	public readonly Player _player;

	private Label Name { get; set; }
	private Label HealthIndicator { get; set; }
	private Label Role { get; set; }

	public Nameplate( Player player ) => _player = player;

	public override void Tick()
	{
		if ( !_player.IsValid() )
			return;

		var health = _player.Health / _player.MaxHealth * 100;
		var healthGroup = _player.GetHealthGroup( health );

		HealthIndicator.Style.FontColor = healthGroup.Color;
		HealthIndicator.Text = healthGroup.Title;

		Name.Text = _player.Client?.Name ?? "";
		if ( _player.Role is not NoneRole and not Innocent )
		{
			Role.Text = _player.Role.Title;
			Role.Style.FontColor = _player.Role.Color;
		}
	}
}
