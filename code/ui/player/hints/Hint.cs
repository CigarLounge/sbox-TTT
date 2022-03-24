using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Hint : EntityHintPanel
{
	private readonly Label _label;

	public Hint( string text )
	{
		StyleSheet.Load( "/ui/player/hints/Hint.scss" );

		AddClass( "centered-vertical-75 background-color-primary rounded opacity-heavy text-shadow" );

		_label = Add.Label( text );
		_label.Style.Padding = 10;

		this.Enabled( false );
	}

	public void SetText( string text ) => _label.Text = text;
}
