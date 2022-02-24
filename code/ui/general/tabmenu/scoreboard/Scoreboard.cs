using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class Scoreboard : Panel
{
	public enum DefaultScoreboardGroup
	{
		Alive,
		Missing,
		Dead,
		Spectator
	}

	public static Scoreboard Instance;

	private readonly Dictionary<Client, ScoreboardEntry> _entries = new();
	private readonly Dictionary<string, ScoreboardGroup> _scoreboardGroups = new();
	private readonly Dictionary<Client, bool> _forcedSpecList = new();

	private readonly Panel _backgroundPanel;
	private readonly Panel _scoreboardContainer;
	private readonly ScoreboardHeader _scoreboardHeader;
	private readonly Panel _scoreboardContent;
	private readonly Panel _scoreboardFooter;

	public Scoreboard( Panel parent, Button swapButton ) : base()
	{
		Parent = parent;
		Instance = this;

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

		foreach ( DefaultScoreboardGroup defaultScoreboardGroup in Enum.GetValues( typeof( DefaultScoreboardGroup ) ) )
		{
			AddScoreboardGroup( defaultScoreboardGroup.ToString() );
		}
	}

	public ScoreboardEntry AddClient( Client client )
	{
		ScoreboardGroup scoreboardGroup = GetScoreboardGroup( client );
		ScoreboardEntry scoreboardEntry = scoreboardGroup.AddEntry( client );

		scoreboardGroup.GroupMembers++;

		_entries.Add( client, scoreboardEntry );

		scoreboardGroup.UpdateLabel();
		return scoreboardEntry;
	}

	public void UpdateClient( Client client )
	{
		if ( client == null )
			return;

		if ( !_entries.TryGetValue( client, out ScoreboardEntry panel ) )
			return;

		ScoreboardGroup scoreboardGroup = GetScoreboardGroup( client );

		if ( scoreboardGroup.GroupTitle != panel.ScoreboardGroupName )
		{
			// instead of remove and add, move the panel into the right parent
			RemoveClient( client );
			AddClient( client );
		}
		else
		{
			panel.Update();
		}

		_scoreboardHeader.UpdateServerInfo();
		UpdateScoreboardGroups();
	}

	public void RemoveClient( Client client )
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

		// This code sucks. I'm forced to due this because of...
		// https://github.com/Facepunch/sbox-issues/issues/1324
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

		// Due to not having a `client.GetValue` change callback, we have to handle it differently
		foreach ( Client client in Client.All )
		{
			bool newIsForcedSpectator = client.GetValue<bool>( RawStrings.ForcedSpectator );

			if ( !_forcedSpecList.TryGetValue( client, out bool isForcedSpectator ) )
			{
				_forcedSpecList.Add( client, newIsForcedSpectator );
			}
			else if ( isForcedSpectator != newIsForcedSpectator )
			{
				_forcedSpecList[client] = newIsForcedSpectator;
			}

			UpdateClient( client );
		}
	}

	private ScoreboardGroup AddScoreboardGroup( string groupName )
	{
		if ( _scoreboardGroups.ContainsKey( groupName ) )
		{
			return _scoreboardGroups[groupName];
		}

		ScoreboardGroup scoreboardGroup = new( _scoreboardContent, groupName );
		scoreboardGroup.UpdateLabel();

		_scoreboardGroups.Add( groupName, scoreboardGroup );

		return scoreboardGroup;
	}

	private ScoreboardGroup GetScoreboardGroup( Client client )
	{
		string group = DefaultScoreboardGroup.Alive.ToString();

		if ( client.GetValue<bool>( RawStrings.ForcedSpectator ) )
		{
			group = DefaultScoreboardGroup.Spectator.ToString();
		}
		else if ( client.PlayerId != 0 && client.Pawn is Player player )
		{
			if ( player.IsConfirmed )
			{
				group = DefaultScoreboardGroup.Dead.ToString();
			}
			else if ( player.IsMissingInAction )
			{
				group = DefaultScoreboardGroup.Missing.ToString();
			}
		}

		_scoreboardGroups.TryGetValue( group, out ScoreboardGroup scoreboardGroup );

		return scoreboardGroup ?? AddScoreboardGroup( group );
	}

	private void UpdateScoreboardGroups()
	{
		foreach ( ScoreboardGroup value in _scoreboardGroups.Values )
		{
			value.Style.Display = value.GroupMembers == 0 ? DisplayMode.None : DisplayMode.Flex;
		}
	}
}
