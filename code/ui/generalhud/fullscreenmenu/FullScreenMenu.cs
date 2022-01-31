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

			AddClass( "background-color-secondary" );
			AddClass( "opacity-medium" );
			AddClass( "fullscreen" );

			this.Style.ZIndex = 2;

			this.Enabled( false );
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
			this.Enabled( true );
		}

		public void Close()
		{
			this.Enabled( false );
			DeleteChildren( true );
			ActivePanel = null;
		}
	}
}