using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

[UseTemplate]
public partial class Scoreboard : Panel
{
	private readonly Dictionary<Client, ScoreboardEntry> _entries = new();
	private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();

	private readonly string _alive = "Alive";
	private readonly string _missingInAction = "Missing In Action";
	private readonly string _confirmedDead = "Confirmed Dead";
	private readonly string _spectator = "Spectator";

	private Panel Container { get; set; }
	private ScoreboardHeader Header { get; set; }
	private Panel Content { get; set; }

	public Scoreboard( Panel parent, Button swapButton ) : base()
	{
		Parent = parent;

		Container.AddChild( swapButton );

		AddScoreboardGroup( _alive );
		AddScoreboardGroup( _missingInAction );
		AddScoreboardGroup( _confirmedDead );
		AddScoreboardGroup( _spectator );
	}

	public void AddClient( Client client )
	{
		var scoreboardGroup = GetScoreboardGroup( client );
		var scoreboardEntry = scoreboardGroup.AddEntry( client );
		scoreboardGroup.GroupMembers++;

		_entries.Add( client, scoreboardEntry );
	}

	private void UpdateClient( Client client )
	{
		if ( client is null )
			return;

		if ( !_entries.TryGetValue( client, out ScoreboardEntry panel ) )
			return;

		var scoreboardGroup = GetScoreboardGroup( client );
		if ( scoreboardGroup.GroupTitle != panel.ScoreboardGroupName )
		{
			RemoveClient( client );
			AddClient( client );
		}
		else
		{
			panel.Update();
		}

		Header.UpdateServerInfo();

		foreach ( var value in _scoreboardGroups.Values )
			value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
	}

	private void RemoveClient( Client client )
	{
		if ( !_entries.TryGetValue( client, out ScoreboardEntry panel ) )
			return;

		_scoreboardGroups.TryGetValue( panel.ScoreboardGroupName, out ScoreboardGroup scoreboardGroup );

		if ( scoreboardGroup is not null )
			scoreboardGroup.GroupMembers--;

		panel.Delete();
		_entries.Remove( client );
	}

	public override void Tick()
	{
		base.Tick();

		if ( !HasClass( "fade-in" ) )
			return;

		foreach ( var client in Client.All.Except( _entries.Keys ) )
		{
			AddClient( client );
			UpdateClient( client );
		}

		foreach ( var client in _entries.Keys.Except( Client.All ) )
		{
			if ( _entries.TryGetValue( client, out var row ) )
			{
				row?.Delete();
				RemoveClient( client );
			}
		}

		foreach ( var client in Client.All )
			UpdateClient( client );
	}

	private ScoreboardGroup AddScoreboardGroup( string groupName )
	{
		var scoreboardGroup = new ScoreboardGroup( Content, groupName );
		_scoreboardGroups.Add( groupName, scoreboardGroup );
		return scoreboardGroup;
	}

	private ScoreboardGroup GetScoreboardGroup( Client client )
	{
		var player = client.Pawn as Player;

		if ( player.IsMissingInAction )
			return _scoreboardGroups[_missingInAction];

		if ( player.IsConfirmedDead )
			return _scoreboardGroups[_confirmedDead];

		return _scoreboardGroups[player.IsSpectator ? _spectator : _alive];
	}
}
