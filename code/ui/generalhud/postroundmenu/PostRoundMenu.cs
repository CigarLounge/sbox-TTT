using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class PostRoundStats
{
	public readonly string WinningRole;
	public Color WinningColor;

	public PostRoundStats( string winningRole, Color winningColor )
	{
		WinningRole = winningRole;
		WinningColor = winningColor;
	}
}

public class PostRoundMenu : Panel
{
	public static PostRoundMenu Instance;

	private PostRoundStats _stats;

	private readonly Panel _backgroundBannerPanel;
	private readonly Panel _containerPanel;

	private readonly Label _headerLabel;
	private readonly Label _contentLabel;

	public PostRoundMenu()
	{
		Instance = this;

		StyleSheet.Load( "/ui/generalhud/postroundmenu/PostRoundMenu.scss" );

		AddClass( "text-shadow" );

		_backgroundBannerPanel = new( this );
		_backgroundBannerPanel.AddClass( "background-color-secondary" );
		_backgroundBannerPanel.AddClass( "background-banner-panel" );
		_backgroundBannerPanel.AddClass( "opacity-medium" );

		_containerPanel = new( _backgroundBannerPanel );
		_containerPanel.AddClass( "container-panel" );

		_headerLabel = _containerPanel.Add.Label();
		_headerLabel.AddClass( "header-label" );

		_contentLabel = _containerPanel.Add.Label();
		_contentLabel.AddClass( "content-label" );
	}

	public void OpenAndSetPostRoundMenu( PostRoundStats stats )
	{
		_stats = stats;

		OpenPostRoundMenu();
	}

	public void ClosePostRoundMenu()
	{
		SetClass( "fade-in", false );
		_containerPanel.SetClass( "pop-in", false );
	}

	public void OpenPostRoundMenu()
	{
		SetClass( "fade-in", true );
		_containerPanel.SetClass( "pop-in", true );

		_contentLabel.Text = "Thanks for playing TTT, more updates and stats to come!";

		_headerLabel.Text = _stats.WinningRole == "nones" ? "IT'S A TIE!" : $"THE {_stats.WinningRole.ToUpper()} WIN!";
		_headerLabel.Style.FontColor = _stats.WinningColor;
	}
}
