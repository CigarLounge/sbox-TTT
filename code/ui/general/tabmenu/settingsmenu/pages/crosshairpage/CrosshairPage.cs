using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class CrosshairPage : Panel
{
	public bool ShowTop { get; set; } = false;
	public bool ShowDot { get; set; } = false;

	public int Size { get; set; } = 0;
	public int Thickness { get; set; } = 0;
	public int Gap { get; set; } = 0;

	public CrosshairPage()
	{
	}

	public override void Tick()
	{
		Crosshair.Instance?.SetupCrosshair( new UI.Crosshair.Properties(
			ShowTop,
			ShowDot,
			false,
			Size,
			Thickness,
			0,
			Gap
		) );
	}
}
