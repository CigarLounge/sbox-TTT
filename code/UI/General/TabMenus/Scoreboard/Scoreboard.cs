using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public class Scoreboard : Panel
{
	private readonly Dictionary<IClient, ScoreboardEntry> _entries = new();
	private readonly Dictionary<PlayerStatus, ScoreboardGroup> _scoreboardGroups = new();

	private Panel Container { get; init; }
	private Panel Content { get; init; }

	public Scoreboard( Panel parent, Panel buttons ) : base()
	{
		Parent = parent;

		Container.AddChild( buttons );

		AddScoreboardGroup( PlayerStatus.Alive );
		AddScoreboardGroup( PlayerStatus.MissingInAction );
		AddScoreboardGroup( PlayerStatus.ConfirmedDead );
		AddScoreboardGroup( PlayerStatus.Spectator );
	}

	public void AddClient( IClient client )
	{
		var scoreboardGroup = GetScoreboardGroup( client );
		var scoreboardEntry = scoreboardGroup.AddEntry( client );
		scoreboardGroup.GroupMembers++;
		_entries.Add( client, scoreboardEntry );
	}

	private void UpdateClient( IClient client )
	{
		if ( client is null )
			return;

		if ( !_entries.TryGetValue( client, out var panel ) )
			return;

		var scoreboardGroup = GetScoreboardGroup( client );
		if ( scoreboardGroup.GroupStatus != panel.PlayerStatus )
		{
			RemoveClient( client );
			AddClient( client );
		}

		foreach ( var value in _scoreboardGroups.Values )
			value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
	}

	private void RemoveClient( IClient client )
	{
		if ( !_entries.TryGetValue( client, out var panel ) )
			return;

		_scoreboardGroups.TryGetValue( panel.PlayerStatus, out var scoreboardGroup );

		if ( scoreboardGroup is not null )
			scoreboardGroup.GroupMembers--;

		panel.Delete();
		_entries.Remove( client );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !IsVisible )
			return;

		foreach ( var client in Game.Clients.Except( _entries.Keys ) )
		{
			AddClient( client );
			UpdateClient( client );
		}

		foreach ( var client in _entries.Keys.Except( Game.Clients ) )
		{
			if ( _entries.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				RemoveClient( client );
			}
		}

		foreach ( var client in Game.Clients )
			UpdateClient( client );
	}

	private ScoreboardGroup AddScoreboardGroup( PlayerStatus someState )
	{
		var scoreboardGroup = new ScoreboardGroup( Content, someState );
		_scoreboardGroups.Add( someState, scoreboardGroup );
		return scoreboardGroup;
	}

	private ScoreboardGroup GetScoreboardGroup( IClient client )
	{
		var player = client.Pawn as Player;

		return _scoreboardGroups[player.Status];
	}
}
