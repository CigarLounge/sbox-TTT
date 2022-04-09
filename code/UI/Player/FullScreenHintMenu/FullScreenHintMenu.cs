using Sandbox.UI;

namespace TTT.UI;

public class FullScreenHintMenu : Panel
{
	public static FullScreenHintMenu Instance;
	public bool IsOpen { get => _activePanel is not null; }

	private Panel _activePanel;
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
		_activePanel = null;

		Open( panel );
	}

	/// <summary>
	/// Ensure that the panel parameter isn't being created on each tick.
	/// </summary>
	public void Open( Panel panel )
	{
		if ( _activePanel is not null )
			return;

		DeleteChildren( true );
		_activePanel = panel;
		AddChild( panel );
		this.EnableFade( true );
	}

	public void Close()
	{
		if ( _activePanel is null || _isForcedOpen ) return;
		this.EnableFade( false );
		DeleteChildren( true );
		_activePanel = null;
		_isForcedOpen = false;
	}
}
