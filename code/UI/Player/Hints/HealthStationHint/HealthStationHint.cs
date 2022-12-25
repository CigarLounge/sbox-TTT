using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class HealthStationHint : Panel
{
	private readonly HealthStationEntity _healthStation;
	public HealthStationHint() { }
	public HealthStationHint( HealthStationEntity healthStation ) => _healthStation = healthStation;
	protected override int BuildHash() => HashCode.Combine( _healthStation.StoredHealth );
}
