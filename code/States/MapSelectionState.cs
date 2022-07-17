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

	public static async Task<IEnumerable<string>> GetMapIdents()
	{
		var query = new Package.Query
		{
			Type = Package.Type.Map,
			Order = Package.Order.User,
			Take = 99,
		};

		query.Tags.Add( "game:" + Global.GameIdent );

		var packages = await query.RunAsync( default );
		return packages.Select( ( p ) => p.FullIdent );
	}

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		if ( Votes.Count == 0 )
		{
			_ = SelectRandomMap();
			return;
		}

		Global.ChangeLevel
		(
			Votes.GroupBy( x => x.Value )
			.OrderBy( x => x.Count() )
			.First().Key
		);
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( Host.IsClient )
			UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapVotePanel() );
	}

	private async Task SelectRandomMap()
	{
		var mapIdents = await GetMapIdents();
		if ( !mapIdents.Any() )
			Global.ChangeLevel( Game.DefaultMap );
		else
			Global.ChangeLevel( mapIdents.ElementAt( Rand.Int( 0, mapIdents.Count() - 1 ) ) );
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
}
