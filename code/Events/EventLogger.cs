using System.Collections.Generic;
using Sandbox;

namespace TTT;

public enum EventType
{
	Round,
	Player
}

public struct EventInfo
{
	public EventType EventType { get; set; }
	public float Time { get; set; }
}

public partial class EventLogger
{
	// s&box doesn't allow for string types in structs, so we need a seperate array for them meanwhile...
	public readonly List<EventInfo> Events = new();
	public readonly List<string> EventDescriptions = new();

	public EventLogger()
	{
		Event.Register( this );
	}

	~EventLogger()
	{
		Event.Unregister( this );
	}

	private void LogEvent( EventType eventType, float time, string description )
	{
		if ( Host.IsClient )
			return;

		EventInfo eventInfo = new()
		{
			EventType = eventType,
			Time = time,
		};

		Events.Add( eventInfo );
		EventDescriptions.Add( description );
	}

	[TTTEvent.Round.Started]
	private void OnRoundStart()
	{
		Events.Clear();
		EventDescriptions.Clear();

		LogEvent( EventType.Round, Game.Current.State.TimeLeft, "The round started." );
	}

	[TTTEvent.Round.Ended]
	private void OnRoundEnd( Team winningTeam, WinType winType )
	{
		LogEvent( EventType.Round, Game.Current.State.TimeLeft, $"The {winningTeam.GetTitle()} won the round!" );
	}

	[TTTEvent.Player.CorpseFound]
	private void OnCorpseFound( Player deadPlayer )
	{
		LogEvent( EventType.Round, Game.Current.State.TimeLeft, $"{deadPlayer.Confirmer.Client.Name} found the corpse of {deadPlayer.Corpse.PlayerName}" );
	}

	[TTTEvent.Player.CreditsFound]
	private void OnCreditsFound( Player player, int creditsFound )
	{
		LogEvent( EventType.Round, Game.Current.State.TimeLeft, $"{player.Client.Name} found {creditsFound} credits." );
	}
}
