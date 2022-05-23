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
		if ( !Host.IsServer )
			return;

		Events.Clear();
		EventDescriptions.Clear();

		LogEvent( EventType.Round, Game.InProgressTime, "The round started." );
	}

	[TTTEvent.Round.Ended]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( !Host.IsServer )
			return;

		LogEvent( EventType.Round, Events[^1].Time, $"The {winningTeam.GetTitle()} won the round!" );
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( Game.Current.State is not InProgress )
			return;

		if ( !player.DiedBySuicide )
			LogEvent( EventType.PlayerKill, Game.Current.State.TimeLeft, $"{player.Client.Name} was killed by {player.LastAttacker.Client.Name}" );
		else if ( player.LastDamageInfo.Flags == DamageFlags.Fall )
			LogEvent( EventType.PlayerSuicide, Game.Current.State.TimeLeft, $"{player.Client.Name} fell to their death." );
	}

	[TTTEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player player )
	{
		if ( !Host.IsServer )
			return;

		LogEvent( EventType.PlayerFind, Game.Current.State.TimeLeft, $"{player.Confirmer.Client.Name} found the corpse of {player.Corpse.PlayerName}" );
	}
}
