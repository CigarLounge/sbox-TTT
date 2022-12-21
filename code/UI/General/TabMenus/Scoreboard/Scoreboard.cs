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
			AddEntry( client );

		// Remove any players from the scoreboard if they've disconnected.
		foreach ( var client in _entries.Keys.Except( Game.Clients ) )
			RemoveEntry( client );

		foreach ( var client in Game.Clients )
		{
			if ( client.Pawn is not Player player || !_entries.ContainsKey( client ) )
				continue;

			var group = _statusGroups[player.Status];
			var entry = _entries[player.Client];

			if ( group.Status != entry.Status )
			{
				RemoveEntry( client );
				AddEntry( client );
			}
		}
	}

	private void AddEntry( IClient client )
	{
		if ( client.Pawn is not Player player )
			return;

		var group = _statusGroups[player.Status];
		var entry = new ScoreboardEntry() { Player = player, Status = player.Status };

		group.AddChild( entry );
		group.Players += 1;

		_entries.Add( player.Client, entry );
	}

	private void RemoveEntry( IClient client )
	{
		if ( !_entries.ContainsKey( client.Client ) )
			return;

		var entry = _entries[client.Client];
		var group = _statusGroups[entry.Status];

		group.Players -= 1;

		entry.Delete();
		_entries.Remove( client.Client );
	}
}
