using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class Crosshair : Panel
{
	public const string FilePath = "crosshair.json";

	public class Properties
	{
		public bool IsDynamic { get; set; }
		public bool ShowTop { get; set; }
		public bool ShowDot { get; set; }
		public int Size { get; set; }
		public int Thickness { get; set; }
		public int Gap { get; set; }
		public Color Color { get; set; }

		public Properties(
			bool IsDynamic,
			bool ShowTop,
			bool ShowDot,
			int Size,
			int Thickness,
			int Gap,
			Color Color )
		{
			this.IsDynamic = IsDynamic;
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
		Config = GetActiveConfig();
	}

	public void RenderCrosshair( Vector2 center, Entity activeChild )
	{
		var draw = Render.Draw2D;
		draw.Color = Config.Color;

		if ( Config.ShowDot )
			draw.Box( new Rect( center.x - (Config.Thickness / 2), center.y - (Config.Thickness / 2), Config.Thickness, Config.Thickness ) );

		var shootEase = 0f;
		if ( Config.IsDynamic && activeChild is Weapon weapon )
			shootEase = Easing.EaseIn( ((float)weapon.TimeSincePrimaryAttack).LerpInverse( 0.2f, 0.0f ) * 5 );

		var startingOffset = Config.Thickness + Config.Gap + shootEase;
		var endingOffset = startingOffset + Config.Size;

		if ( Config.ShowTop )
			draw.Line( Config.Thickness, center - Vector2.Up * startingOffset, center - Vector2.Up * endingOffset );

		draw.Line( Config.Thickness, center + Vector2.Up * startingOffset, center + Vector2.Up * endingOffset );
		draw.Line( Config.Thickness, center + Vector2.Left * startingOffset, center + Vector2.Left * endingOffset );
		draw.Line( Config.Thickness, center - Vector2.Left * startingOffset, center - Vector2.Left * endingOffset );
	}

	public static Properties GetActiveConfig()
	{
		return FileSystem.Data.ReadJson<Properties>( FilePath ) ?? Instance?.Config ?? new Properties( true, true, true, 0, 5, 0, Color.White );
	}
}
