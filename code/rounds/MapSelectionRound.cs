using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTT;

public partial class MapSelectionRound : BaseRound
{
	[Net, Change] public IDictionary<string, string> MapImages { get; set; }
	[Net] public IDictionary<long, string> PlayerIdVote { get; set; }

	public override string RoundName => "Map Selection";
	public override int RoundDuration => Game.MapSelectionTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		// We failed to fetch TTT maps, fall back to default map.
		if ( MapImages.Count == 0 )
		{
			Log.Warning( "No viable TTT-support maps found on server. Restarting game on default map." );
			Global.ChangeLevel( Game.DefaultMap );
			return;
		}

		var mapToVoteCount = GetTotalVotesPerMap();

		// Nobody voted, so let's change to a random map.
		if ( mapToVoteCount.Count == 0 )
		{
			Global.ChangeLevel( MapImages.ElementAt( Rand.Int( 0, MapImages.Count - 1 ) ).Key );
			return;
		}

		// Change to the map which received the most votes first.
		Global.ChangeLevel( mapToVoteCount.OrderByDescending( x => x.Value ).First().Key );
	}

	public override void OnPlayerKilled( Player player )
	{
		base.OnPlayerKilled( player );

		player.MakeSpectator();
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( Host.IsClient )
		{
			UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapSelectionMenu() );
			return;
		}

		MapImages = new Dictionary<string, string>();
		PlayerIdVote = new Dictionary<long, string>();
		_ = Load();
	}

	[ServerCmd]
	public static void SetVote( string map )
	{
		long callerPlayerId = ConsoleSystem.Caller.PlayerId;
		IDictionary<long, string> nextMapVotes = (Game.Current.Round as MapSelectionRound).PlayerIdVote;

		nextMapVotes[callerPlayerId] = map;
	}

	private async Task Load()
	{
		List<string> mapNames = await GetMapNames();
		List<string> mapImages = await GetMapImages( mapNames );

		for ( int i = 0; i < mapNames.Count; ++i )
		{
			MapImages[mapNames[i]] = mapImages[i];
		}
	}

	private async Task<List<string>> GetMapNames()
	{
		Package result = await Package.Fetch( RawStrings.GameIndent, true );
		return result?.GameConfiguration?.MapList ?? new List<string>();
	}

	private async Task<List<string>> GetMapImages( List<string> mapNames )
	{
		List<string> mapPanels = new();

		for ( int i = 0; i < mapNames.Count; ++i )
		{
			Package result = await Package.Fetch( mapNames[i], true );
			mapPanels.Add( result.Thumb );
		}

		return mapPanels;
	}

	public IDictionary<string, int> GetTotalVotesPerMap()
	{
		IDictionary<string, int> indexToVoteCount = new Dictionary<string, int>();

		foreach ( string mapName in (Game.Current.Round as MapSelectionRound)?.PlayerIdVote.Values )
		{
			indexToVoteCount[mapName] = !indexToVoteCount.ContainsKey( mapName ) ? 1 : indexToVoteCount[mapName] + 1;
		}

		return indexToVoteCount;
	}

	private void OnMapImagesChanged()
	{
		UI.MapSelectionMenu.Instance?.InitMapPanels();
	}
}
