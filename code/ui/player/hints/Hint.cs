using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class Hint : EntityHintPanel
{
	private readonly Label _label;
	private readonly Label _subLabel;

	public Hint( string text, string subtext = "" )
	{
		StyleSheet.Load( "/ui/player/hints/Hint.scss" );

		AddClass( "centered-vertical-75 background-color-primary rounded opacity-heavy text-shadow" );

		_label = Add.Label( text );
		_label.Style.Padding = 10;

		if ( !string.IsNullOrEmpty( subtext ) )
		{
			_label.Style.PaddingBottom = 5;
			_subLabel = Add.Label( subtext, "sublabel" );
		}

		this.Enabled( false );
	}

	public override void UpdateHintPanel( string text, string subtext = "" )
	{
		_label.Text = text;

		if ( string.IsNullOrEmpty( subtext ) )
			_subLabel?.Delete();
		else if ( _subLabel != null )
			_subLabel.Text = subtext;
	}
}
