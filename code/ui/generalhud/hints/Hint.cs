using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class Hint : EntityHintPanel
	{
		private readonly Label _label;

		public Hint( string text )
		{
			AddClass( "centered-vertical-75" );
			AddClass( "background-color-primary" );
			AddClass( "rounded" );
			AddClass( "text-color-info" );
			AddClass( "text-shadow" );

			_label = Add.Label( text );
			_label.Style.Padding = 10;

			this.Enabled( false );
		}

		public override void UpdateHintPanel( string text )
		{
			_label.Text = text;
		}
	}
}
