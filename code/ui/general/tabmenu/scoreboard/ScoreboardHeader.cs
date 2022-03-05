using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class ScoreboardHeader : Panel
{
	private readonly Panel _gameLogoPanel;
	private readonly Panel _serverInfoPanel;
	private readonly Label _serverNameLabel;
	private readonly Label _serverDescriptionLabel;
	private readonly Panel _serverDataPanel;
	private readonly Label _serverMapLabel;
	private readonly Label _serverPlayersLabel;

	public ScoreboardHeader( Panel parent ) : base( parent )
	{
		AddClass( "text-shadow" );

		_gameLogoPanel = new( this );
		_gameLogoPanel.AddClass( "game-logo" );

		_serverInfoPanel = new( this );
		_serverInfoPanel.AddClass( "server-information-panel" );

		_serverNameLabel = _serverInfoPanel.Add.Label();
		_serverNameLabel.AddClass( "server-name-label" );
		_serverNameLabel.Text = "TTT";

		_serverDescriptionLabel = _serverInfoPanel.Add.Label();
		_serverDescriptionLabel.AddClass( "server-description-label" );
		_serverDescriptionLabel.Text = "Created by github.com/mzegar/sbox-TTT/contributors";

		_serverDataPanel = new( this );
		_serverDataPanel.AddClass( "server-data-panel" );

		_serverMapLabel = _serverDataPanel.Add.Label();
		_serverMapLabel.AddClass( "server-map-label" );

		_serverPlayersLabel = _serverDataPanel.Add.Label();
		_serverPlayersLabel.AddClass( "server-players-label" );

		UpdateServerInfo();
	}

	public void UpdateServerInfo()
	{
		int maxPlayers = ConsoleSystem.GetValue( "maxplayers" ).ToInt( 0 );

		_serverMapLabel.Text = Global.MapName;
		_serverPlayersLabel.Text = $"{Client.All.Count} / {maxPlayers} Players";
	}
}
