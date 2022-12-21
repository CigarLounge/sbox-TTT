using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	public Panel Buttons { get; set; }
	private Panel GroupPanel { get; set; }
	private readonly Dictionary<PlayerStatus, ScoreboardGroup> _statusGroups = new();
	private readonly Dictionary<IClient, ScoreboardEntry> _entries = new();

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
		if ( !IsVisible )
			return;

		// Add any players to the scoreboard if they aren't there yet.
		foreach ( var client in Game.Clients.Except( _entries.Keys ) )
			AddEntry( client.Pawn as Player );

		// Remove any players from the scoreboard if they've disconnected.
		foreach ( var client in _entries.Keys.Except( Game.Clients ) )
			RemoveEntry( client.Pawn as Player );

		foreach ( var client in Game.Clients )
		{
			if ( !_entries.ContainsKey( client ) )
				continue;

			var player = client.Pawn as Player;
			var group = _statusGroups[player.Status];
			var entry = _entries[player.Client];

			if ( group.Status != entry.Status )
			{
				RemoveEntry( player );
				AddEntry( player );
			}
		}
	}

	private void AddEntry( Player player )
	{
		var group = _statusGroups[player.Status];
		var entry = new ScoreboardEntry() { Player = player, Status = player.Status };

		group.AddChild( entry );
		group.Players += 1;

		_entries.Add( player.Client, entry );
	}

	private void RemoveEntry( Player player )
	{
		if ( !_entries.ContainsKey( player.Client ) )
			return;

		var entry = _entries[player.Client];
		var group = _statusGroups[entry.Status];

		group.Players -= 1;

		entry.Delete();
		_entries.Remove( player.Client );
	}
}
