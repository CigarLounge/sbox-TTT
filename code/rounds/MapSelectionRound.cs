using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace TTT;

public class MapSelectionRound : BaseRound
{
	public override string RoundName => "Map Selection";
	public override int RoundDuration => Game.MapSelectionTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		IDictionary<string, string> maps = Game.Current.MapSelection.MapImages;

		// We failed to fetch TTT maps, fall back to default map.
		if ( maps.Count == 0 )
		{
			Log.Warning( "No viable TTT-support maps found on server. Restarting game on default map." );
			Global.ChangeLevel( Game.DefaultMap );
			return;
		}

		IDictionary<long, string> playerIdMapVote = Game.Current.MapSelection.PlayerIdMapVote;
		IDictionary<string, int> mapToVoteCount = MapSelectionHandler.GetTotalVotesPerMap( playerIdMapVote );

		// Nobody voted, so let's change to a random map.
		if ( mapToVoteCount.Count == 0 )
		{
			Global.ChangeLevel( maps.ElementAt( Rand.Int( 0, maps.Count - 1 ) ).Key );
			return;
		}

		// Change to the map which received the most votes first.
		Global.ChangeLevel( mapToVoteCount.OrderByDescending( x => x.Value ).First().Key );
	}

	public override void OnPlayerKilled( Player player )
	{
		player.MakeSpectator();
	}

	protected override void OnStart()
	{
		if ( Host.IsClient )
			UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapSelectionMenu() );
	}
}
