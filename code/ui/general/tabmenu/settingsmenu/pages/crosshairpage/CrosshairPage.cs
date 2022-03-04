using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class CrosshairPage : Panel
{
	public bool ShowTop { get; set; } = true;
	public bool ShowDot { get; set; } = false;

	public int Size { get; set; } = 7;
	public int Thickness { get; set; } = 3;
	public int Gap { get; set; } = -5;
	public int OutlineThickness { get; set; } = 0;

	public int R { get; set; } = 255;
	public int G { get; set; } = 255;
	public int B { get; set; } = 255;
	public int A { get; set; } = 255;

	public CrosshairPage()
	{

	}

	public override void Tick()
	{
		Crosshair.Instance?.SetupCrosshair( new UI.Crosshair.Properties(
			ShowTop,
			ShowDot,
			Size,
			Thickness,
			OutlineThickness,
			Gap,
			Color.FromBytes( R, G, B, A )
		) );
	}
}
