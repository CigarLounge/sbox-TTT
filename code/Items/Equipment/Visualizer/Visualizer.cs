namespace TTT;

// WIP, not currently added to any shop.
[Category( "Equipment" )]
[ClassName( "ttt_equipment_visualizer" )]
[Title( "Visualizer" )]
public class Visualizer : Deployable<VisualizerEntity>
{
	protected override bool CanPlant => false;
}
