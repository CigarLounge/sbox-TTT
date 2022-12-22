using Sandbox;
using Sandbox.UI;
using System;
using System.Text.Json;

namespace TTT.UI;

public partial class QuickChat : Panel
{
	public class TargetInfo
	{
		public string Name { get; set; } = NoTarget;
		public bool IsPlayerName { get; set; } = false;
	}

	public static QuickChat Instance { get; private set; }

	public bool IsShowing { get; private set; } = false;

	private const string NoTarget = "Nobody";
	private TargetInfo _target = new();
	private RealTimeSince _timeWithNoTarget;
	private RealTimeSince _timeSinceLastMessage;

	public QuickChat() => Instance = this;

	public override void Tick()
	{
		if ( Input.Pressed( InputButton.Zoom ) )
			IsShowing = !IsShowing;

		var newTarget = GetTarget();

		var hasTarget = newTarget.Name != NoTarget || newTarget.IsPlayerName;
		if ( hasTarget )
			_timeWithNoTarget = 0;
		else if ( _timeWithNoTarget <= 1 )
			return;

		if ( newTarget == _target )
			return;

		_target = newTarget;
	}

	private TargetInfo GetTarget()
	{
		if ( Game.LocalPawn is not Player localPlayer )
			return new TargetInfo();

		switch ( localPlayer.HoveredEntity )
		{
			case Corpse corpse:
			{
				if ( corpse.Player is null )
					return new TargetInfo() { Name = "an unidentified body", IsPlayerName = false };
				else
					return new TargetInfo() { Name = $"{corpse.Player.SteamName}'s corpse", IsPlayerName = false };
			}
			case Player player:
			{
				if ( player.CanHint( localPlayer ) )
					return new TargetInfo() { Name = player.SteamName, IsPlayerName = true };
				else
					return new TargetInfo() { Name = "someone in disguise", IsPlayerName = false };
			}
		}

		return new TargetInfo();
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
			TextChat.SendQuickChat( entry.Prefix, entry.TargetInfo.Name, entry.Postfix, entry.TargetInfo.IsPlayerName );
			_timeSinceLastMessage = 0;
		}

		IsShowing = false;
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( IsShowing, Game.LocalPawn.IsAlive(), _target, _target.Name, _target.IsPlayerName );
	}
}
