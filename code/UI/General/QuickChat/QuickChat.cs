using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class QuickChat : Panel
{
	public bool IsShowing { get; set; }

	private readonly List<string> _messages = new(){
		"1: Yes.",
		"2: No.",
		"3: Help!",
		"4: I'm with {0}.",
		"5: I see {0}.",
		"6: {0} acts suspicious.",
		"7: {0} is a Traitor!",
		"8: {0} is innocent.",
		"9: Anyone still alive?",
	};

	private readonly List<Label> _labels = new();

	public QuickChat()
	{
		foreach ( var message in _messages )
			_labels.Add( Add.Label( message ) );
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( Input.Pressed( InputButton.Zoom ) )
			IsShowing = !IsShowing;

		this.Enabled( player.IsAlive() && IsShowing );
		if ( !this.IsEnabled() )
			return;

		for ( var i = 0; i < _labels.Count; ++i )
			_labels[i].Text = string.Format( _messages[i], player.LastSeenPlayerName );
	}
}
