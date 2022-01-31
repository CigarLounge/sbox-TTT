using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI
{
	public class FullScreenMenu : Panel
	{
		public static FullScreenMenu Instance;
		public Panel ActivePanel { get; private set; }

		public FullScreenMenu()
		{
			Instance = this;

			StyleSheet.Load( "/ui/generalhud/fullscreenmenu/FullScreenMenu.scss" );

			AddClass( "background-color-secondary" );
			AddClass( "fullscreen" );

			this.Style.ZIndex = 2;

			this.EnableFade( false );
		}

		public void Open( Panel panel )
		{
			if ( ActivePanel != null )
			{
				return;
			}

			DeleteChildren( true );
			ActivePanel = panel;
			AddChild( panel );
			this.EnableFade( true );
		}

		public void Close()
		{
			this.EnableFade( false );
			DeleteChildren( true );
			ActivePanel = null;
		}
	}
}