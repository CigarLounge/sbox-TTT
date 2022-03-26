using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class InputGlyph : Panel
{
	private InputButton _inputButton;
	private GlyphStyle _glyphStyle;

	public InputGlyph()
	{
		StyleSheet.Load( "/ui/components/inputglyph/InputGlyph.scss" );
	}

	public void SetButton( InputButton inputButton )
	{
		_inputButton = inputButton;

		var texture = Input.GetGlyph( _inputButton, InputGlyphSize.Small, _glyphStyle );
		Style.BackgroundImage = texture;
		Style.Width = texture.Width;
		Style.Height = texture.Height;
	}

	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "button":
			{
				InputButton.TryParse( value, true, out _inputButton );
				SetButton( _inputButton );

				break;
			}
			case "style":
			{
				_glyphStyle = value switch
				{
					"knockout" => GlyphStyle.Knockout,
					"light" => GlyphStyle.Light,
					"dark" => GlyphStyle.Dark,
					_ => _glyphStyle
				};

				SetButton( _inputButton );
				break;
			}
		}

		base.SetProperty( name, value );
	}
}
