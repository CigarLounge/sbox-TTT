using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace TTT;

public partial class MapSelectionState : BaseState
{
	[Net]
	public IDictionary<Client, string> Votes { get; private set; }

	public override string Name => "Map Selection";
	public override int Duration => Game.MapSelectionTime;

	protected override void OnTimeUp()
	{
		base.OnTimeUp();

		if ( Votes.Count == 0 )
		{
			Global.ChangeLevel( Game.DefaultMap );
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

	private void CullInvalidClients()
	{
		foreach ( var entry in Votes.Keys.Where( x => !x.IsValid() ).ToArray() )
		{
			Votes.Remove( entry );
		}
	}

	[ServerCmd]
	public static void SetVote( string map )
	{
		if ( Game.Current.State is not MapSelectionState state )
			return;

		if ( ConsoleSystem.Caller.Pawn is not Player player )
			return;

		state.CullInvalidClients();
		state.Votes[player.Client] = map;
	}
}
