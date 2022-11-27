using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTT;

public partial class MapSelectionState : BaseState
{
	[Net]
	public IDictionary<Client, string> Votes { get; private set; }

	public override string Name { get; } = "Map Selection";
	public override int Duration => Game.MapSelectionTime;

	public const string MapsFile = "maps.txt";

	protected override void OnTimeUp()
	{
		if ( Votes.Count == 0 )
		{
			Global.ChangeLevel( Rand.FromList( Game.Current.MapVoteIdents as List<string> ) ?? Game.DefaultMap );
			return;
		}

		Global.ChangeLevel
		(
			Votes.GroupBy( x => x.Value )
			.OrderBy( x => x.Count() )
			.Last().Key
		);
	}

	protected override void OnStart()
	{
		UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapVotePanel() );
	}

	[ConCmd.Server]
	public static void SetVote( string map )
	{
		if ( Game.Current.State is not MapSelectionState state )
			return;

		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		state.Votes[player.Client] = map;
	}

	[Event.Entity.PostSpawn]
	private static async void OnFinishedLoading()
	{
		var maps = await GetLocalMapIdents();
		if ( maps.IsNullOrEmpty() )
			maps = await GetRemoteMapIdents();

		maps.Shuffle();
		Game.Current.MapVoteIdents = maps;
	}

	private static async Task<List<string>> GetLocalMapIdents()
	{
		var maps = new List<string>();

		var rawMaps = FileSystem.Data.ReadAllText( MapsFile );
		if ( rawMaps.IsNullOrEmpty() )
			return maps;

		var splitMaps = rawMaps.Split( "\n" );
		foreach ( var rawMap in splitMaps )
		{
			var mapIdent = rawMap.Trim();
			var package = await Package.Fetch( mapIdent, true );

			if ( package is not null && package.PackageType == Package.Type.Map )
				maps.Add( mapIdent );
			else
				Log.Error( $"{mapIdent} does not exist as a s&box map!" );
		}

		return maps;
	}

	private static async Task<List<string>> GetRemoteMapIdents()
	{
		var queryResult = await Package.FindAsync( $"type:map game:{Global.GameIdent.Replace( "#local", "" )}", take: 99 );
		return queryResult.Packages.Select( ( p ) => p.FullIdent ).ToList();
	}
}
