using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

/// Taken from sbox UI tests, thanks!
/// <summary>
/// A horizontal slider with a text entry on the right
/// </summary>
public class ColorEditor : Panel
{
	public Slider2D SaturationValueSlider { get; set; }
	protected Panel Canvas { get; set; }
	public Panel Presets { get; protected set; }

	public ColorEditor()
	{
		StyleSheet.Load( "/UI/Components/ColorEditor/ColorEditor.scss" );
		SaturationValueSlider = AddChild<Slider2D>( "satval" );
		SaturationValueSlider.Add.Panel( "value_gradient" );
		SaturationValueSlider.Bind( "value", this, "SaturationAndValue" );

		Canvas = Add.Panel( "canvas" );
		SwitchRGBA();

		Presets = Add.Panel( "presets" );

		AddColorPreset( 0, Color.Parse( "#f44336" ) );
		AddColorPreset( 1, Color.Parse( "#e91e63" ) );
		AddColorPreset( 2, Color.Parse( "#9c27b0" ) );
		AddColorPreset( 3, Color.Parse( "#673ab7" ) );
		AddColorPreset( 4, Color.Parse( "#3f51b5" ) );
		AddColorPreset( 5, Color.Parse( "#2196f3" ) );
		AddColorPreset( 6, Color.Parse( "#03a9f4" ) );
		AddColorPreset( 7, Color.Parse( "#00bcd4" ) );
		AddColorPreset( 8, Color.Parse( "#009688" ) );
		AddColorPreset( 9, Color.Parse( "#4caf50" ) );
		AddColorPreset( 10, Color.Parse( "#8bc34a" ) );
		AddColorPreset( 11, Color.Parse( "#cddc39" ) );
		AddColorPreset( 12, Color.Parse( "#ffeb3b" ) );
		AddColorPreset( 13, Color.Parse( "#ffc107" ) );
		AddColorPreset( 14, Color.Parse( "#ff9800" ) );
		AddColorPreset( 15, Color.Parse( "#ff5722" ) );
		AddColorPreset( 16, Color.Parse( "#795548" ) );
		AddColorPreset( 17, Color.Parse( "#9e9e9e" ) );
		AddColorPreset( 18, Color.Parse( "#607d8b" ) );
		AddColorPreset( 19, Color.White );
		AddColorPreset( 20, Color.Gray );
		AddColorPreset( 21, Color.Black );

		// If you add more it's gonna wrap and look like shit so leave it here
	}

	void AddColorPreset( int num, Color? defVal )
	{
		defVal = Cookie.Get<Color>( $"color_preset_{num}", defVal ?? Color.White );
		var p = Presets.Add.Panel( "preset" );
		p.AddEventListener( "onmousedown", e =>
		{
			if ( e is MousePanelEvent mpe )
			{
				// Left Click = Get From Preset
				if ( mpe.Button == "mouseleft" )
				{
					Value = p.Style.BackgroundColor.Value;
				}

				// Right click = Store To Preset
				if ( mpe.Button == "mouseright" )
				{
					p.Style.BackgroundColor = Value;
					p.Style.Dirty();

					Cookie.Set<Color>( $"color_preset_{num}", Value );
				}
			}
		} );

		p.Style.BackgroundColor = defVal;
	}

	public void SwitchRGBA()
	{
		Canvas.DeleteChildren( true );

		var h = Canvas.Add.Slider( 0, 359, 1 );
		h.AddClass( "hue" );
		h.Bind( "value", this, "Hue" );

		var a = Canvas.Add.Slider( 0, 1, 0.001f );
		a.AddClass( "alpha_slider" );
		a.Bind( "value", this, "AlphaValue" );
		AlphaValue = 1;
	}


	ColorHsv color;

	/// <summary>
	/// The actual value. Setting the value will snap and clamp it.
	/// </summary>
	[Property]
	public ColorHsv Value
	{
		get => color;
		set
		{
			var hsv = value;

			if ( SaturationValueSlider.HasActive && (hsv.Saturation < 0.1f || hsv.Value < 0.1f) && hsv.Hue == 0 )
				hsv = hsv.WithHue( color.Hue );

			if ( color == value ) return;

			color = hsv;
			CreateValueEvent( "value", color.ToColor() );

			SaturationValueSlider.Thumb.Style.BackgroundColor = color.WithAlpha( 1 );
			SaturationValueSlider.Thumb.Style.Dirty();

			SaturationValueSlider.Style.BackgroundColor = new ColorHsv( hsv.Hue, 1, 1 );
			SaturationValueSlider.Style.Dirty();
		}
	}

	public float AlphaValue
	{
		get => Value.Alpha;
		set
		{
			Value = Value.WithAlpha( value );
		}
	}

	public float Hue
	{
		get
		{
			return color.Hue;
		}

		set
		{
			Value = color.WithHue( value );
		}
	}

	public Vector2 SaturationAndValue
	{
		get
		{
			return new Vector2( Value.Saturation, Value.Value );
		}
		set
		{
			Value = Value.WithSaturation( value.x ).WithValue( value.y );
		}
	}
}
