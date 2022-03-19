using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	private readonly Dictionary<Client, ScoreboardEntry> _entries = new();
	private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();

	private readonly string _alive = "Alive";
	private readonly string _missingInAction = "Missing In Action";
	private readonly string _confirmedDead = "Confirmed Dead";
	private readonly string _spectator = "Spectator";

	private readonly Panel _backgroundPanel;
	private readonly Panel _scoreboardContainer;
	private readonly ScoreboardHeader _scoreboardHeader;
	private readonly Panel _scoreboardContent;
	private readonly Panel _scoreboardFooter;

	public Scoreboard( Panel parent, Button swapButton ) : base()
	{
		Parent = parent;

		StyleSheet.Load( "/ui/general/tabmenu/scoreboard/Scoreboard.scss" );

		_backgroundPanel = new( this );
		_backgroundPanel.AddClass( "fullscreen" );

		_scoreboardContainer = new( this );
		_scoreboardContainer.AddClass( "rounded" );
		_scoreboardContainer.AddClass( "scoreboard-container" );

		_scoreboardHeader = new( _scoreboardContainer );
		_scoreboardHeader.AddClass( "background-color-secondary" );
		_scoreboardHeader.AddClass( "rounded-top" );

		_scoreboardContent = new( _scoreboardContainer );
		_scoreboardContent.AddClass( "background-color-primary" );
		_scoreboardContent.AddClass( "scoreboard-content" );

		_scoreboardFooter = new( _scoreboardContainer );
		_scoreboardFooter.AddClass( "background-color-secondary" );
		_scoreboardFooter.AddClass( "scoreboard-footer" );
		_scoreboardFooter.AddClass( "rounded-bottom" );

		_scoreboardContainer.AddChild( swapButton );

		Initialize();
	}

	[Event.Hotload]
	private void Initialize()
	{
		if ( Host.IsServer )
			return;

		AddScoreboardGroup( _alive );
		AddScoreboardGroup( _missingInAction );
		AddScoreboardGroup( _confirmedDead );
		AddScoreboardGroup( _spectator );
	}

	public void AddClient( Client client )
	{
		ScoreboardGroup scoreboardGroup = GetScoreboardGroup( client );
		ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry( client );
		scoreboardGroup.UpdateLabel();
		scoreboardGroup.GroupMembers++;

		_entries.Add( client, scoreboardEntry );
	}

	private void UpdateClient( Client client )
	{
		if ( client == null )
			return;

		if ( !_entries.TryGetValue( client, out ScoreboardEntry panel ) )
			return;

		ScoreboardGroup scoreboardGroup = GetScoreboardGroup( client );
		if ( scoreboardGroup.GroupTitle != panel.ScoreboardGroupName )
		{
			RemoveClient( client );
			AddClient( client );
		}
		else
		{
			panel.Update();
		}

		_scoreboardHeader.UpdateServerInfo();

		foreach ( ScoreboardGroup value in _scoreboardGroups.Values )
			value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
	}

	private void RemoveClient( Client client )
	{
		if ( !_entries.TryGetValue( client, out ScoreboardEntry panel ) )
			return;

		_scoreboardGroups.TryGetValue( panel.ScoreboardGroupName, out ScoreboardGroup scoreboardGroup );

		if ( scoreboardGroup != null )
			scoreboardGroup.GroupMembers--;

		scoreboardGroup.UpdateLabel();

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

		foreach ( Client client in Client.All )
			UpdateClient( client );
	}

	private ScoreboardGroup AddScoreboardGroup( string groupName )
	{
		if ( _scoreboardGroups.ContainsKey( groupName ) )
			return _scoreboardGroups[groupName];

		ScoreboardGroup scoreboardGroup = new( _scoreboardContent, groupName );
		scoreboardGroup.UpdateLabel();

		_scoreboardGroups.Add( groupName, scoreboardGroup );

		return scoreboardGroup;
	}

	private ScoreboardGroup GetScoreboardGroup( Client client )
	{
		if ( client.Pawn is Player player )
		{
			if ( player.IsMissingInAction )
				return _scoreboardGroups[_missingInAction];

			if ( player.IsConfirmedDead )
				return _scoreboardGroups[_confirmedDead];
		}

		return _scoreboardGroups[client.GetValue<bool>( RawStrings.Spectator ) ? _spectator : _alive];
	}
}
