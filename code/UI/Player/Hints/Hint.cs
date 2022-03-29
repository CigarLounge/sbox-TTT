using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public abstract class EntityHintPanel : Panel
{
}

public class Hint : EntityHintPanel
{
	private readonly Label _label;

	public Hint( string text )
	{
		StyleSheet.Load( "/ui/player/hints/Hint.scss" );

		AddClass( "centered-vertical-75 text-shadow" );

		_label = Add.Label( text );

		this.Enabled( false );
	}

	public void SetText( string text ) => _label.Text = text;
}
