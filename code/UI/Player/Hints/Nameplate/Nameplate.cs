using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class Nameplate : EntityHintPanel
{
	public readonly Player _player;

	private Label Name { get; init; }
	private Label HealthStatus { get; init; }
	private Label KarmaStatus { get; init; }
	private Label Role { get; init; }

	public Nameplate( Player player ) => _player = player;

	public override void Tick()
	{
		if ( !_player.IsValid() )
			return;

		var health = _player.Health / _player.MaxHealth * 100;
		var healthGroup = _player.GetHealthGroup( health );

		HealthStatus.Style.FontColor = healthGroup.Color;
		HealthStatus.Text = healthGroup.Title;

		KarmaStatus.Text = Karma.GetKarmaGroup( _player );

		Name.Text = _player.Client?.Name ?? "";
		if ( _player.Role is not NoneRole and not Innocent )
		{
			Role.Text = _player.Role.Title;
			Role.Style.FontColor = _player.Role.Color;
		}
	}
}
