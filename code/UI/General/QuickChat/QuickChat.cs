using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class QuickChat : Panel
{
	public static QuickChat Instance;

	private string _target;
	private string _cachedTarget;

	private bool _isShowing = false;
	private TimeSince _timeSinceLastMessage;

	private readonly List<Label> _labels = new();
	private static readonly List<string> _messages = new()
	{
		"{0} is a Traitor!",
		"{0} acts suspicious.",
		"I'm with {0}.",
		"I see {0}.",
		"Yes.",
		"No.",
		"{0} is Innocent.",
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

		_target = GetTarget();
		if ( _target == _cachedTarget )
			return;

		_cachedTarget = _target;
		for ( var i = 0; i < _labels.Count; ++i )
			_labels[i].Text = $"{i + 1}: {string.Format( _messages[i], _target )}";
	}

	public static string GetTarget()
	{
		if ( Local.Pawn is not Player localPlayer )
			return "nobody";

		switch ( localPlayer.HoveredEntity )
		{
			case Corpse corpse:
			{
				if ( corpse.Player is null )
					return "an unidentified body";
				else
					return $"{corpse.Player.Client.Name}'s corpse";
			}
			case Player player:
			{
				if ( player.CanHint( localPlayer ) )
					return player.Client.Name;
				else
					return "someone in disguise";
			}
		}

		return "nobody";
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
			ChatBox.SendChat( string.Format( _messages[keyboardIndexPressed - 1], _target ) );
			_timeSinceLastMessage = 0;
		}

		_isShowing = false;
	}
}
