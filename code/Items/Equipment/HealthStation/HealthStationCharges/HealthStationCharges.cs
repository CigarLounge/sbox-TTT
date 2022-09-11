using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class HealthStationCharges : WorldPanel
{
	private readonly HealthStationEntity _healthStation;

	private Label Label { get; init; }

	public HealthStationCharges( HealthStationEntity healthStation ) => _healthStation = healthStation;

	public override void Tick()
	{
		base.Tick();

		SceneObject.Transform = _healthStation.GetAttachment( "charges" ) ?? default;
		Label.Text = $"{(int)_healthStation.StoredHealth}";
	}
}
