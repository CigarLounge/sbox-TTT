namespace TTT;

[Category( "Equipment" )]
[ClassName( "ttt_equipment_healthstation" )]
[Title( "Health Station" )]
public class HealthStation : Deployable<HealthStationEntity>
{
	protected override bool CanPlant => false;
}
