using Sandbox;
using Sandbox.UI;

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
		return FileSystem.Data.ReadJson<Properties>( FilePath ) ?? Instance?.Config ?? new Properties( true, true, true, 0, 5, 0, Color.White );
	}

	public override void Tick()
	{
		this.Enabled( Game.LocalPawn.IsAlive() );
	}
}
