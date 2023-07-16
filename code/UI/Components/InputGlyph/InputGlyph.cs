using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public class InputGlyph : Panel
{
	private string _inputAction;
	private InputGlyphSize _inputGlyphSize;
	private GlyphStyle _glyphStyle;

	public InputGlyph()
	{
		StyleSheet.Load( "/UI/Components/InputGlyph/InputGlyph.scss" );
	}

	public void SetButton( string inputButton )
	{
		if ( _inputAction == inputButton )
			return;

		_inputAction = inputButton;
		Update();
	}

	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "action":
			{
				
				SetButton( _inputAction );
				Update();

				break;
			}
			case "size":
			{
				InputGlyphSize.TryParse( value, true, out _inputGlyphSize );
				Update();

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

				Update();
				break;
			}
		}

		base.SetProperty( name, value );
	}

	private void Update()
	{
		var texture = Input.GetGlyph( _inputAction, _inputGlyphSize, _glyphStyle );
		Style.BackgroundImage = texture;
		Style.Width = texture.Width;
		Style.Height = texture.Height;
	}
}
