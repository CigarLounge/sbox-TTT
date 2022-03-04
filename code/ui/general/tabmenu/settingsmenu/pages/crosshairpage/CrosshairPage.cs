using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class CrosshairPage : Panel
{
	public bool ShowTop { get; set; } = true;
	public bool ShowDot { get; set; } = false;

	public int Size { get; set; } = 4;
	public int Thickness { get; set; } = 3;
	public int Gap { get; set; } = 3;
	public int OutlineThickness { get; set; } = 0;

	public int R { get; set; } = 255;
	public int G { get; set; } = 255;
	public int B { get; set; } = 255;
	public int A { get; set; } = 255;

	public CrosshairPage()
	{
		var crosshairConfig = FileSystem.Data.ReadJson<Crosshair.Properties>( "crosshair.json" );
		if ( crosshairConfig == null )
			return;

		ShowTop = crosshairConfig.ShowTop;
		ShowDot = crosshairConfig.ShowDot;
		Size = crosshairConfig.Size;
		Thickness = crosshairConfig.Thickness;
		Gap = crosshairConfig.Gap;
		OutlineThickness = crosshairConfig.OutlineThickness;
		R = (int)crosshairConfig.Color.r * 255;
		G = (int)crosshairConfig.Color.g * 255;
		B = (int)crosshairConfig.Color.b * 255;
		A = (int)crosshairConfig.Color.a * 255;
	}

	public void SaveCrosshairData()
	{
		if ( Crosshair.Instance != null )
			FileSystem.Data.WriteJson( "crosshair.json", Crosshair.Instance.Config );
		SettingsMenu.Instance.PopPage();
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
