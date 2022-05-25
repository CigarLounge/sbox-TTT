using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class Crosshair : Panel
{
	public class Properties
	{
		public bool ShowTop { get; set; }
		public bool ShowDot { get; set; }
		public int Size { get; set; }
		public int Thickness { get; set; }
		public int Gap { get; set; }
		public Color Color { get; set; }

		public Properties(
			bool ShowTop,
			bool ShowDot,
			int Size,
			int Thickness,
			int Gap,
			Color Color )
		{
			this.ShowTop = ShowTop;
			this.ShowDot = ShowDot;
			this.Size = Size;
			this.Thickness = Thickness;
			this.Gap = Gap;
			this.Color = Color;
		}
	}

	public static Crosshair Instance;
	public Properties Config { get; set; }

	public Crosshair()
	{
		Instance = this;
		Config = FileSystem.Data.ReadJson<Properties>( "crosshair.json" ) ?? new Properties( false, true, 0, 5, 0, Color.White );
	}

	public void RenderCrosshair( Vector2 center )
	{
		var draw = Render.Draw2D;
		draw.Color = Config.Color;

		if ( Config.ShowDot )
			draw.Box( new Rect( center.x - (Config.Thickness / 2), center.y - (Config.Thickness / 2), Config.Thickness, Config.Thickness ) );

		var startingOffset = Config.Thickness + Config.Gap;
		var endingOffset = Config.Thickness + Config.Gap + Config.Size;

		if ( Config.ShowTop )
			draw.Line( Config.Thickness, center - Vector2.Up * startingOffset, center - Vector2.Up * endingOffset );

		draw.Line( Config.Thickness, center + Vector2.Up * startingOffset, center + Vector2.Up * endingOffset );
		draw.Line( Config.Thickness, center + Vector2.Left * startingOffset, center + Vector2.Left * endingOffset );
		draw.Line( Config.Thickness, center - Vector2.Left * startingOffset, center - Vector2.Left * endingOffset );
	}
}
