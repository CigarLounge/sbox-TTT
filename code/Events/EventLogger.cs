using System.Collections.Generic;
using Sandbox;

namespace TTT;

public enum EventType
{
	Round,
	PlayerKill,
	PlayerSuicide,
	PlayerFind
}

public struct EventInfo
{
	public EventType EventType { get; set; }
	public float Time { get; set; }
}

public static class EventLogger
{
	// s&box doesn't allow for string types in structs, so we need a seperate array for them meanwhile...
	public static readonly List<EventInfo> Events = new();
	public static readonly List<string> EventDescriptions = new();

	private static void LogEvent( EventType eventType, float time, string description )
	{
		EventInfo eventInfo = new()
		{
			EventType = eventType,
			Time = time,
		};

		Events.Add( eventInfo );
		EventDescriptions.Add( description );
	}

	[TTTEvent.Round.Started]
	private static void OnRoundStart()
	{
		Events.Clear();
		EventDescriptions.Clear();

		LogEvent( EventType.Round, Game.InProgressTime, "The round started." );
	}

	[TTTEvent.Round.Ended]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( Events.Count != 0 )
			LogEvent( EventType.Round, Events[^1].Time, $"The {winningTeam.GetTitle()} won the round!" );
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player deadPlayer )
	{
		if ( deadPlayer.LastDamageInfo.Attacker is Player attacker )
			LogEvent( EventType.PlayerKill, Game.Current.State.TimeLeft, $"{deadPlayer.Client.Name} was killed by {attacker.Client.Name}" );
		else if ( deadPlayer.LastDamageInfo.Flags == DamageFlags.Fall )
			LogEvent( EventType.PlayerSuicide, Game.Current.State.TimeLeft, $"{deadPlayer.Client.Name} fell to their death." );
	}

	[TTTEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player deadPlayer )
	{
		LogEvent( EventType.PlayerFind, Game.Current.State.TimeLeft, $"{deadPlayer.Confirmer.Client.Name} found the corpse of {deadPlayer.Corpse.PlayerName}" );
	}

	[TTTEvent.Player.CreditsFound]
	private static void OnCreditsFound( Player player, int creditsFound )
	{
		LogEvent( EventType.PlayerFind, Game.Current.State.TimeLeft, $"{player.Client.Name} found {creditsFound} credits." );
	}
}
