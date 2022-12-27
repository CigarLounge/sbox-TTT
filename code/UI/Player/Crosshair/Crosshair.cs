using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.Utility;

namespace TTT.UI;

public partial class Crosshair : Panel
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

	public static Properties GetActiveConfig()
	{
		return FileSystem.Data.ReadJson<Properties>( FilePath ) ?? Instance?.Config ?? new Properties( true, true, true, 0, 4, 0, Color.White );
	}

	public override void DrawBackground( ref RenderState state )
	{
		if ( Game.LocalPawn is not Player player || !player.IsAlive() )
			return;

		var center = Screen.Size / 2;

		var shootEase = 0f;
		if ( Config.IsDynamic && player.ActiveCarriable is Weapon weapon )
			shootEase = Easing.EaseIn( ((float)weapon.TimeSincePrimaryAttack).LerpInverse( 0.2f, 0.0f ) * 5 );

		var startingOffset = Config.Gap + shootEase;
		var endingOffset = startingOffset + Config.Size;

		if ( Config.ShowDot )
			Draw.Line( Config.Color, Config.Thickness, center + Vector2.Up * Config.Thickness / 2, Config.Thickness, center - Vector2.Up * Config.Thickness / 2 );

		if ( Config.ShowTop )
			Draw.Line( Config.Color, Config.Thickness, center - Vector2.Up * startingOffset, Config.Thickness, center - Vector2.Up * endingOffset );

		Draw.Line( Config.Color, Config.Thickness, center + Vector2.Up * startingOffset, Config.Thickness, center + Vector2.Up * endingOffset );
		Draw.Line( Config.Color, Config.Thickness, center + Vector2.Left * startingOffset, Config.Thickness, center + Vector2.Left * endingOffset );
		Draw.Line( Config.Color, Config.Thickness, center - Vector2.Left * startingOffset, Config.Thickness, center - Vector2.Left * endingOffset );
	}
}
