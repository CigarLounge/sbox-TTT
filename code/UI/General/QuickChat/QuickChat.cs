using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class QuickChat : Panel
{
	public static QuickChat Instance;

	private string _currentPlayerName { get => !string.IsNullOrEmpty( _cachedLastPlayerSeen ) ? _cachedLastPlayerSeen : "Nobody"; }
	private string _cachedLastPlayerSeen;

	private bool _isShowing = false;
	private TimeSince _timeSinceLastMessage;

	private readonly List<Label> _labels = new();
	private readonly List<string> _messages = new(){
		"{0} acts suspicious.",
		"{0} is a Traitor!",
		"{0} is innocent.",
		"I'm with {0}.",
		"I see {0}.",
		"Yes.",
		"No.",
		"Help!",
		"Anyone still alive?"
	};

	public QuickChat()
	{
		Instance = this;

		foreach ( var message in _messages )
			_labels.Add( Add.Label( message ) );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( Input.Pressed( InputButton.Zoom ) )
			_isShowing = !_isShowing;

		this.Enabled( player.IsAlive() && _isShowing );
		if ( !this.IsEnabled() )
			return;

		if ( player.LastSeenPlayerName == _cachedLastPlayerSeen )
			return;

		_cachedLastPlayerSeen = player.LastSeenPlayerName;
		for ( var i = 0; i < _labels.Count; ++i )
			_labels[i].Text = $"{i + 1}: {string.Format( _messages[i], _currentPlayerName )}";
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder input )
	{
		if ( !this.IsEnabled() )
			return;

		var keyboardIndexPressed = InventorySelection.GetKeyboardNumberPressed( input );
		if ( keyboardIndexPressed <= 0 ) // Only accept keyboard numbers 1-9
			return;

		if ( _timeSinceLastMessage > 1 )
		{
			ChatBox.SendChat( string.Format( _messages[keyboardIndexPressed - 1], _currentPlayerName ) );
			_timeSinceLastMessage = 0;
		}

		_isShowing = false;
	}
}
