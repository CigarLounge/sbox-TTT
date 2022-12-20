using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	public Panel Buttons { get; set; }
	private Panel GroupPanel { get; set; }
	private readonly Dictionary<PlayerStatus, ScoreboardGroup> _statusGroups = new();
	private readonly Dictionary<Player, ScoreboardGroup> _clientGroups = new();

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		foreach ( var group in GroupPanel.ChildrenOfType<ScoreboardGroup>() )
			_statusGroups.Add( group.Status, group );

		AddChild( Buttons );
	}

	public override void Tick()
	{
		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn is not Player player )
				continue;

			if ( !_clientGroups.ContainsKey( player ) )
			{
				_clientGroups.Add( player, _statusGroups[player.Status] );
				_clientGroups[player].Players.Add( player );
				continue;
			}

			var currentGroup = _clientGroups[player];
			if ( player.Status != currentGroup.Status )
			{
				currentGroup.Players.Remove( player );
				_clientGroups.Remove( player );
			}
		}
	}
}
