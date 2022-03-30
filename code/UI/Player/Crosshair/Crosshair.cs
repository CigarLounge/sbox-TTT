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
		public int OutlineThickness { get; set; }
		public int Gap { get; set; }
		public Color Color { get; set; }

		public Properties(
			bool ShowTop,
			bool ShowDot,
			int Size,
			int Thickness,
			int OutlineThickness,
			int Gap,
			Color Color )
		{
			this.ShowTop = ShowTop;
			this.ShowDot = ShowDot;
			this.Size = Size;
			this.Thickness = Thickness;
			this.OutlineThickness = OutlineThickness;
			this.Gap = Gap;
			this.Color = Color;
		}
	}

	public static Crosshair Instance;
	public Properties Config { get; set; }

	private readonly List<Panel> _lines = new();
	private readonly Panel _topLine;
	private readonly Panel _leftLine;
	private readonly Panel _rightLine;
	private readonly Panel _bottomLine;
	private readonly Panel _dot;

	public Crosshair()
	{
		Instance = this;
		StyleSheet.Load( "/UI/Player/Crosshair/Crosshair.scss" );
		_topLine = Add.Panel( "line" );
		_leftLine = Add.Panel( "line" );
		_rightLine = Add.Panel( "line" );
		_bottomLine = Add.Panel( "line" );
		_dot = Add.Panel( "dot" );

		_lines.Add( _leftLine );
		_lines.Add( _bottomLine );
		_lines.Add( _rightLine );
		_lines.Add( _topLine );

		var crosshairConfig = FileSystem.Data.ReadJson<Properties>( "crosshair.json" );
		SetupCrosshair( crosshairConfig ?? new Properties( false, true, 0, 5, 0, 0, Color.White ) );
	}

	public void SetupCrosshair( Properties crosshairProperties )
	{
		for ( int i = 0; i < _lines.Count; ++i )
		{
			var isHorizontal = i % 2 == 0;
			var crosshairLine = _lines[i];

			crosshairLine.Style.BackgroundColor = crosshairProperties.Color;
			crosshairLine.Style.Width = isHorizontal
				? crosshairProperties.Size
				: crosshairProperties.Thickness;
			crosshairLine.Style.Height = isHorizontal ? crosshairProperties.Thickness : crosshairProperties.Size;
			crosshairLine.Style.BorderColor = Color.Black;
			crosshairLine.Style.BorderWidth = crosshairProperties.OutlineThickness;

			switch ( i )
			{
				case 0: // Left element
					crosshairLine.Style.Left = Length.Pixels( crosshairProperties.Size + crosshairProperties.Gap );
					break;
				case 1: // Bottom element
					crosshairLine.Style.Top = Length.Pixels( crosshairProperties.Size + crosshairProperties.Gap );
					break;
				case 2: // Right element
					crosshairLine.Style.Left = Length.Pixels( -crosshairProperties.Size - crosshairProperties.Gap );
					break;
				case 3: // Top element
					crosshairLine.Style.Top = Length.Pixels( -crosshairProperties.Size - crosshairProperties.Gap );
					break;
			}
		}

		if ( crosshairProperties.ShowDot )
		{
			_dot.Style.BackgroundColor = crosshairProperties.Color;
			_dot.Style.Width = crosshairProperties.Thickness;
			_dot.Style.Height = crosshairProperties.Thickness;
			_dot.Style.Opacity = crosshairProperties.Color.a;
			_dot.Style.BorderColor = Color.Black;
			_dot.Style.BorderWidth = crosshairProperties.OutlineThickness;
		}
		else
		{
			_dot.Style.Opacity = 0;
		}

		_topLine.Style.Opacity = crosshairProperties.ShowTop ? crosshairProperties.Color.a : 0;

		Config = crosshairProperties;
	}

	public override void Tick()
	{
		this.Enabled( Local.Pawn.IsAlive() );
	}
}
