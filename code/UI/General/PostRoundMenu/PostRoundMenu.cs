using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class PostRoundMenu : Panel
{
	public static PostRoundMenu Instance;

	private readonly Panel _backgroundBannerPanel;
	private readonly Panel _containerPanel;

	private readonly Label _headerLabel;
	private readonly Label _contentLabel;

	private Team _winningTeam;

	public PostRoundMenu()
	{
		Instance = this;

		StyleSheet.Load( "/UI/General/PostRoundMenu/PostRoundMenu.scss" );

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

	[ClientRpc]
	public static void DisplayWinner( Team team )
	{
		Instance._winningTeam = team;

		Instance.Open();
	}

	public void Close()
	{
		SetClass( "fade-in", false );
		_containerPanel.SetClass( "pop-in", false );
	}

	public void Open()
	{
		SetClass( "fade-in", true );
		_containerPanel.SetClass( "pop-in", true );

		_contentLabel.Text = "Thanks for playing TTT, more updates and stats to come!";

		_headerLabel.Text = _winningTeam == Team.None ? "IT'S A TIE!" : $"THE {_winningTeam.GetTitle()} WIN!";
		_headerLabel.Style.FontColor = _winningTeam.GetColor();
	}
}
