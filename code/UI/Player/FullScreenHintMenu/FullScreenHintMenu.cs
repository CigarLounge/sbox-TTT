using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class FullScreenHintMenu : Panel
{
	public static FullScreenHintMenu Instance;
	public bool IsOpen { get => ActivePanel is not null; }
	public Panel ActivePanel { get; private set; }

	private bool _isForcedOpen = false;

	public FullScreenHintMenu()
	{
		Instance = this;
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
		if ( ActivePanel is not null )
			return;

		DeleteChildren( true );
		ActivePanel = panel;
		AddChild( panel );
		this.EnableFade( true );
	}

	public void Close()
	{
		if ( ActivePanel is null || _isForcedOpen )
			return;

		this.EnableFade( false );
		DeleteChildren( true );
		ActivePanel = null;
		_isForcedOpen = false;
	}
}
