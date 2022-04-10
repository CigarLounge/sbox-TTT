using Sandbox.UI;
using TTT.Entities;

namespace TTT.UI;

[UseTemplate]
public class HealthStationHint : EntityHintPanel
{
	private readonly HealthStation _healthStation;
	private Label Charges { get; set; }

	public HealthStationHint() { }

	public HealthStationHint( HealthStation healthStation )
	{
		_healthStation = healthStation;
	}

	public override void Tick()
	{
		base.Tick();

		Charges.Text = $"{(int)_healthStation.StoredHealth} charges remaining.";
	}
}
