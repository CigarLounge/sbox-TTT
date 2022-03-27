using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class ScoreboardHeader : Panel
{
	private readonly Panel _terryImage;
	private readonly Panel _serverInfoWrapper;
	private readonly Label _serverName;
	private readonly Label _serverDescription;
	private readonly Panel _serverInfo;
	private readonly Label _playerCount;
	private readonly Label _currentMap;

	public ScoreboardHeader( Panel parent ) : base( parent )
	{
		AddClass( "text-shadow" );

		_terryImage = new( this );
		_terryImage.AddClass( "terry-image" );

		_serverInfoWrapper = new( this );
		_serverInfoWrapper.AddClass( "server-information-panel" );

		_serverName = _serverInfoWrapper.Add.Label();
		_serverName.AddClass( "server-name-label" );
		_serverName.Text = "TTT";

		_serverDescription = _serverInfoWrapper.Add.Label();
		_serverDescription.AddClass( "server-description-label" );
		_serverDescription.Text = "Created by github.com/mzegar/sbox-TTT";

		_serverInfo = new( this );
		_serverInfo.AddClass( "server-data-panel" );

		_playerCount = _serverInfo.Add.Label();
		_playerCount.AddClass( "server-players-label" );

		_currentMap = _serverInfo.Add.Label();
		_currentMap.AddClass( "server-map-label" );

		UpdateServerInfo();
	}

	public void UpdateServerInfo()
	{
		int maxPlayers = ConsoleSystem.GetValue( "maxplayers" ).ToInt( 0 );

		_currentMap.Text = Global.MapName;
		_playerCount.Text = $"{Client.All.Count} / {maxPlayers} Players";
	}
}
