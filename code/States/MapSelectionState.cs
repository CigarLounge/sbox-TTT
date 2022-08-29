using Sandbox;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TTT;

public partial class MapSelectionState : BaseState
{
	[Net, Change]
	public static IList<string> MapIdents { get; private set; }

	[Net]
	public IDictionary<Client, string> Votes { get; private set; }

	public override string Name { get; } = "Map Selection";
	public override int Duration => Game.MapSelectionTime;

	protected override void OnTimeUp()
	{
		if ( Votes.Count == 0 )
		{
			Global.ChangeLevel( !MapIdents.IsNullOrEmpty() ? MapIdents.ElementAt( Rand.Int( 0, MapIdents.Count - 1 ) ) : Game.DefaultMap );
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

	private static List<string> GetLocalMapIdents()
	{
		var maps = new List<string>();

		var rawMaps = FileSystem.Data.ReadAllText( "maps.txt" );
		if ( rawMaps.IsNullOrEmpty() )
			return maps;

		var splitMaps = rawMaps.Split( "\n" );
		foreach ( var map in splitMaps )
			maps.Add( map.Trim() );

		return maps;
	}

	private static async Task<List<string>> GetRemoteMapIdents()
	{
		var query = new Package.Query
		{
			Type = Package.Type.Map,
			Order = Package.Order.User,
			Take = 99,
		};

		query.Tags.Add( "game:" + Global.GameIdent );

		var packages = await query.RunAsync( default );
		return packages.Select( ( p ) => p.FullIdent ).ToList();
	}

	private static void OnMapIdentsChange( IList<string> old, IList<string> newValue )
	{
		Log.Info( newValue );
	}

	[Event.Entity.PostSpawn]
	private static async void OnFinishedLoading()
	{
		var maps = GetLocalMapIdents();
		if ( maps.IsNullOrEmpty() )
			maps = await GetRemoteMapIdents();

		MapIdents = maps;
	}
}
