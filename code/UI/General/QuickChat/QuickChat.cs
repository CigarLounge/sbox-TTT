using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace TTT.UI;

[UseTemplate]
public class QuickChat : Panel
{
	public static QuickChat Instance;

	private const string NoTarget = "nobody";
	private string _target;
	private bool _isShowing = false;
	private RealTimeSince _timeWithNoTarget;
	private RealTimeSince _timeSinceLastMessage;

	private readonly List<Label> _labels = new();
	private static readonly List<string> _messages = new()
	{
		"I'm with {0}.",
		"I see {0}.",
		"Yes.",
		"No.",
		"{0} is a Traitor!",
		"{0} acts suspicious.",
		"{0} is Innocent.",
		"Help!",
		"Anyone still alive?"
	};

	public QuickChat()
	{
		Instance = this;

		var i = 0;
		foreach ( Label label in Children )
		{
			_labels.Add( label.Add.Label( _messages[i++], "message" ) );
		}
	}

	public override void Tick()
	{
		if ( Local.Pawn is not Player player )
			return;

		if ( Input.Pressed( InputButton.Zoom ) )
			_isShowing = !_isShowing;

		this.Enabled( player.IsAlive() && _isShowing );

		var newTarget = GetTarget();

		if ( newTarget != NoTarget )
			_timeWithNoTarget = 0;
		else if ( _timeWithNoTarget <= 3 )
			return;

		if ( newTarget == _target )
			return;

		_target = newTarget;
		for ( var i = 0; i <= 6; i++ )
		{
			if ( i == 2 || i == 3 )
				continue;

			_labels[i].Text = string.Format( _messages[i], _target );

			if ( i < 4 )
				continue;

			if ( !ShouldCapitalize( _target ) )
				continue;

			_labels[i].Text = _labels[i].Text.FirstCharToUpper();
		}
	}

	public static string GetTarget()
	{
		if ( Local.Pawn is not Player localPlayer )
			return null;

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

		return NoTarget;
	}

	private static bool ShouldCapitalize( string target )
	{
		return target == NoTarget || target == "an unidentified body" || target == "someone in disguise";
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
			ChatBox.SendChat( _labels[keyboardIndexPressed - 1].Text );
			_timeSinceLastMessage = 0;
		}

		_isShowing = false;
	}
}
