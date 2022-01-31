using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class FullScreenMenu : Panel
	{
		public static FullScreenMenu Instance;

		public FullScreenMenu()
		{
			Instance = this;

			AddClass( "background-color-secondary" );
			AddClass( "opacity-medium" );
			AddClass( "fullscreen" );

			this.Style.ZIndex = 2;

			this.Enabled( false );
		}

		public void OpenMenu( Panel panel )
		{
			DeleteChildren( true );
			AddChild( panel );
			this.Enabled( true );
		}
	}
}