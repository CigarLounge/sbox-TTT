using Sandbox.UI;

namespace TTT.UI;

public class FullScreenHintMenu : Panel
{
	public static FullScreenHintMenu Instance;
	public Panel ActivePanel { get; private set; }
	public bool IsForcedOpen { get; set; } = false;

	public FullScreenHintMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/player/fullscreenHintmenu/FullScreenHintMenu.scss" );

		AddClass( "background-color-secondary" );
		AddClass( "fullscreen" );

		Style.ZIndex = 2;

		this.EnableFade( false );
	}

	public void ForceOpen( Panel panel )
	{
		IsForcedOpen = true;
		ActivePanel = null;

		Open( panel );
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
		if ( ActivePanel == null || IsForcedOpen ) return;
		this.EnableFade( false );
		DeleteChildren( true );
		ActivePanel = null;
		IsForcedOpen = false;
	}
}
