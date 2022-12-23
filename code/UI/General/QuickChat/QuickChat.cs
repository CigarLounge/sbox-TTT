using Sandbox;
using Sandbox.UI;
using System;
using System.Text.Json;

namespace TTT.UI;

public partial class QuickChat : Panel
{
	public class NoTarget : IQuickChatTarget
	{
		public string Message => "Nobody";
	}

	public static bool IsShowing { get; private set; } = false;

	private IQuickChatTarget _target;
	private RealTimeSince _timeSinceLastTarget;
	private RealTimeSince _timeSinceLastMessage;

	public override void Tick()
	{
		if ( Input.Pressed( InputButton.Zoom ) )
			IsShowing = !IsShowing;

		var newTarget = (Game.LocalPawn as Player)?.HoveredEntity as IQuickChatTarget ?? new NoTarget();

		if ( newTarget is not NoTarget )
			_timeSinceLastTarget = 0;
		else if ( _timeSinceLastTarget <= 1 )
			return;

		if ( newTarget == _target )
			return;

		_target = newTarget;
	}

	[Event.Client.BuildInput]
	private void BuildInput()
	{
		if ( !this.IsEnabled() )
			return;

		var keyboardIndexPressed = InventorySelection.GetKeyboardNumberPressed();
		if ( keyboardIndexPressed <= 0 ) // Only accept keyboard numbers 1-9
			return;

		if ( _timeSinceLastMessage > 1 )
		{
			var entry = GetChild( keyboardIndexPressed - 1 ) as QuickChatEntry;
			if ( entry.Target is null )
				TextChat.SendChatMessage( entry.Prefix + entry.Suffix );
			else
				TextChat.SendQuickChat( entry.Prefix, entry.Suffix, entry.Target.Message, JsonSerializer.Serialize( entry.Target.MessageColor ) );

			_timeSinceLastMessage = 0;
		}

		IsShowing = false;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( IsShowing, Game.LocalPawn.IsAlive(), _target, _target?.Message, _target?.MessageColor );
	}
}
