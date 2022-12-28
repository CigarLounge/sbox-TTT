using System;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class CrosshairPage : Panel
{
	private bool IsDynamic { get; set; } = false;
	private bool ShowDot { get; set; } = true;
	private bool ShowTop { get; set; } = true;
	private int Size { get; set; } = 0;
	private int Thickness { get; set; } = 4;
	private int Gap { get; set; } = 0;

	private ColorHsv CurrentColor { get; set; } = Color.White;

	public CrosshairPage()
	{
		var crosshairConfig = Crosshair.GetActiveConfig();
		IsDynamic = crosshairConfig.IsDynamic;
		ShowTop = crosshairConfig.ShowTop;
		ShowDot = crosshairConfig.ShowDot;
		Size = crosshairConfig.Size;
		Thickness = crosshairConfig.Thickness;
		Gap = crosshairConfig.Gap;
		CurrentColor = crosshairConfig.Color;
	}

	public void SaveCrosshairData()
	{
		if ( Crosshair.Instance is not null )
			FileSystem.Data.WriteJson( Crosshair.FilePath, Crosshair.Instance.Config );
	}

	protected override int BuildHash()
	{
		Crosshair.Instance.Config = new Crosshair.Properties( IsDynamic, ShowTop, ShowDot, Size, Thickness, Gap, CurrentColor );
		return HashCode.Combine( IsDynamic, ShowTop, ShowDot, Size, Thickness, Gap, CurrentColor );
	}
}
