using Sandbox.UI;

namespace TTT.UI;

public class FullScreenHintMenu : Panel
{
	public static FullScreenHintMenu Instance;
	public bool IsOpen { get => ActivePanel != null; }
	public Panel ActivePanel { get; private set; }

	private bool _isForcedOpen = false;

	public FullScreenHintMenu()
	{
		Instance = this;

		StyleSheet.Load( "/UI/Player/FullScreenHintMenu/FullScreenHintMenu.scss" );

		AddClass( "background-color-secondary" );
		AddClass( "fullscreen" );

		Style.ZIndex = 2;

		this.EnableFade( false );
	}

	public void ForceOpen( Panel panel )
	{
		_isForcedOpen = true;
		ActivePanel = null;

		Open( panel );
	}

	/// <summary>
	/// Ensure that the panel parameter isn't being created on each tick.
	/// </summary>
	public void Open( Panel panel )
	{
		if ( ActivePanel != null )
			return;

		DeleteChildren( true );
		ActivePanel = panel;
		AddChild( panel );
		this.EnableFade( true );
	}

	public void Close()
	{
		if ( ActivePanel == null || _isForcedOpen ) return;
		this.EnableFade( false );
		DeleteChildren( true );
		ActivePanel = null;
		_isForcedOpen = false;
	}
}
