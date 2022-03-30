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

		StyleSheet.Load( "/UI/General/MapSelectionMenu/MapSelectionMenu.scss" );

		AddClass( "text-shadow" );

		Add.Label( "Vote for the next map!", "title" );

		_mapPanels = new();

		_mapWrapper = new Panel( this );
		_mapWrapper.AddClass( "map-wrapper" );

		InitMapPanels();
	}

	public void InitMapPanels()
	{
		IDictionary<string, string> mapImages = (Game.Current.Round as MapSelectionRound).MapImages;
		foreach ( KeyValuePair<string, string> mapImage in mapImages )
		{
			if ( _mapPanels.Exists( ( mapPanel ) => mapPanel.MapName == mapImage.Key ) )
				continue;

			var panel = new MapPanel( mapImage.Key, mapImage.Value )
			{
				Parent = _mapWrapper
			};

			_mapPanels.Add( panel );
		}
	}

	public override void Tick()
	{
		if ( Game.Current.Round is not MapSelectionRound mapSelectionRound )
			return;

		IDictionary<long, string> playerIdMapVote = mapSelectionRound.PlayerIdVote;

		IDictionary<string, int> mapToVoteCount = mapSelectionRound.GetTotalVotesPerMap();

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
				 MapSelectionRound.SetVote( MapName );
			 } );
		}
	}
}
