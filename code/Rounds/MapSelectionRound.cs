using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public partial class MapSelectionRound : BaseRound
{
	[Net]
	public IDictionary<Client, string> Votes { get; set; }
	public string WinningMap { get; set; } = "facepunch.datacore";

	public override string RoundName => "Map Selection";
	public override int RoundDuration => Game.MapSelectionTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		// We failed to fetch TTT maps, fall back to default map.
		if ( Votes.Count == 0 )
		{
			Log.Warning( "No viable TTT-support maps found on server. Restarting game on default map." );
			Global.ChangeLevel( Game.DefaultMap );
			return;
		}

		Global.ChangeLevel( Votes.GroupBy( x => x.Value ).OrderBy( x => x.Count() ).First().Key );
	}

	protected override void OnStart()
	{
		base.OnStart();

		if ( Host.IsClient )
		{
			// UI.FullScreenHintMenu.Instance?.ForceOpen( new UI.MapSelectionMenu() );
			Local.Hud.AddChild( new UI.MapVotePanel() );
			return;
		}
	}

	public void CullInvalidClients()
	{
		foreach ( var entry in Votes.Keys.Where( x => !x.IsValid() ).ToArray() )
		{
			Votes.Remove( entry );
		}
	}

	[ServerCmd]
	public static void SetVote( string map )
	{
		if ( Game.Current.Round is not MapSelectionRound round || ConsoleSystem.Caller.Pawn is not Player player )
			return;

		round.CullInvalidClients();
		round.Votes[player.Client] = map;
	}
}
