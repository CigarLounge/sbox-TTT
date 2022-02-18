using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public class MapSelectionMenu : Panel
{
	public static MapSelectionMenu Instance;

	private readonly Panel _mapWrapper;
	private readonly List<MapPanel> _mapPanels;

	public MapSelectionMenu() : base()
	{
		Instance = this;

		StyleSheet.Load( "/ui/generalhud/mapselectionmenu/MapSelectionMenu.scss" );

		AddClass( "text-shadow" );

		Add.Label( "Vote for the next map!", "title" );

		_mapPanels = new();

		_mapWrapper = new Panel( this );
		_mapWrapper.AddClass( "map-wrapper" );

		InitMapPanels();
	}

	private void InitMapPanels()
	{
		IDictionary<string, string> mapImages = Gamemode.Game.Instance.MapSelection.MapImages;
		foreach ( KeyValuePair<string, string> mapImage in mapImages )
		{
			if ( _mapPanels.Exists( ( mapPanel ) => mapPanel.MapName == mapImage.Key ) )
			{
				continue;
			}

			MapPanel panel = new( mapImage.Key, mapImage.Value )
			{
				Parent = _mapWrapper
			};

			_mapPanels.Add( panel );
		}
	}

	public override void Tick()
	{
		IDictionary<long, string> playerIdMapVote = Gamemode.Game.Instance.MapSelection.PlayerIdMapVote;

		IDictionary<string, int> mapToVoteCount = Map.MapSelectionHandler.GetTotalVotesPerMap( playerIdMapVote );

		bool hasLocalClientVoted = playerIdMapVote.ContainsKey( Local.Client.PlayerId );

		_mapPanels.ForEach( ( mapPanel ) =>
		 {
			 mapPanel.TotalVotes.Text = mapToVoteCount.ContainsKey( mapPanel.MapName ) ? $"{mapToVoteCount[mapPanel.MapName]}" : string.Empty;

			 mapPanel.SetClass( "voted", hasLocalClientVoted && playerIdMapVote[Local.Client.PlayerId] == mapPanel.MapName );
		 } );
	}

	public class MapPanel : Panel
	{
		public string MapName;
		public Label TotalVotes;

		public MapPanel( string name, string image )
		{
			MapName = name;

			AddClass( "box-shadow" );
			AddClass( "info-panel" );
			AddClass( "rounded" );

			Add.Label( MapName, "map-name" );
			TotalVotes = Add.Label( string.Empty, "map-vote" );

			Style.BackgroundImage = Texture.Load( image );

			AddEventListener( "onclick", () =>
			 {
				 VoteNextMap( MapName );
			 } );
		}
	}

	[ServerCmd( Name = "vote_next_map" )]
	public static void VoteNextMap( string name )
	{
		long callerPlayerId = ConsoleSystem.Caller.PlayerId;
		IDictionary<long, string> nextMapVotes = Gamemode.Game.Instance.MapSelection.PlayerIdMapVote;

		nextMapVotes[callerPlayerId] = name;
		Log.Debug( $"{callerPlayerId} voting for map {name}" );
	}
}
