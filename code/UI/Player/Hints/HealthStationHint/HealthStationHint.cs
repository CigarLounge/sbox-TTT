using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class HealthStationHint : Panel
{
	private readonly HealthStationEntity _healthStation;
	private Label Charges { get; set; }

	public HealthStationHint() { }

	public HealthStationHint( HealthStationEntity healthStation )
	{
		_healthStation = healthStation;
	}

	public override void Tick()
	{
		base.Tick();

		Charges.Text = $"{(int)_healthStation.StoredHealth} charges remaining.";
	}
}
