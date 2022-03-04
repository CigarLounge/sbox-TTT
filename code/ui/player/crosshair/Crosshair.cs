using System.Collections.Generic;
using Sandbox.UI;

namespace TTT.UI;

public class Crosshair : Panel
{
	public class Properties
	{
		public bool ShowTop { get; private set; }
		public bool ShowDot { get; private set; }

		public int Size { get; private set; }
		public int Thickness { get; private set; }
		public int OutlineThickness { get; private set; }

		public int Gap { get; private set; }

		public Color Color { get; private set; }

		public Properties(
			bool showTop = true,
			bool showDot = false,
			int size = 7,
			int thickness = 2,
			int outlineThickness = 0,
			int gap = -5,
			Color? color = null )
		{
			ShowTop = showTop;
			ShowDot = showDot;
			Size = size;
			Thickness = thickness;
			OutlineThickness = outlineThickness;
			Gap = gap;
			Color = color ?? Color.White;
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
		StyleSheet.Load( "/ui/player/crosshair/Crosshair.scss" );

		_topLine = Add.Panel( "line" );
		_leftLine = Add.Panel( "line" );
		_rightLine = Add.Panel( "line" );
		_bottomLine = Add.Panel( "line" );
		_dot = Add.Panel( "dot" );

		_lines.Add( _leftLine );
		_lines.Add( _bottomLine );
		_lines.Add( _rightLine );
		_lines.Add( _topLine );

		SetupCrosshair( new Properties() );
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
}
