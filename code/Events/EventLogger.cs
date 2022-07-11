using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Sandbox;

namespace TTT;

public enum EventType
{
	Round,
	PlayerTookDamage,
	PlayerKill,
	PlayerSuicide,
	PlayerCorpseFound
}

public class EventInfo
{
	public EventType EventType { get; set; }
	public float Time { get; set; }
	public string Description { get; set; }

	public static byte[] Serialize( EventInfo[] data ) => Encoding.UTF8.GetBytes( JsonSerializer.Serialize( data ) );
	public static EventInfo[] Deserialize( byte[] bytes ) => JsonSerializer.Deserialize<EventInfo[]>( bytes );
}

public static class EventLogger
{
	public static readonly List<EventInfo> Events = new();

	private const string LogFolder = "round-logs";

	private static void LogEvent( EventType eventType, float time, string description )
	{
		EventInfo eventInfo = new()
		{
			EventType = eventType,
			Time = time,
			Description = description
		};

		Events.Add( eventInfo );
	}

	[TTTEvent.Round.Started]
	private static void OnRoundStart()
	{
		if ( !Host.IsServer )
			return;

		Events.Clear();

		LogEvent( EventType.Round, Game.InProgressTime, "The round started." );
	}

	[TTTEvent.Round.Ended]
	private static void OnRoundEnd( Team winningTeam, WinType winType )
	{
		if ( !Host.IsServer )
			return;

		LogEvent( EventType.Round, Events[^1].Time, $"The {winningTeam.GetTitle()} won the round!" );

		WriteEvents();
	}

	[TTTEvent.Player.TookDamage]
	private static void OnPlayerTookDamage( Player player )
	{
		if ( !Host.IsServer )
			return;

		var info = player.LastDamageInfo;
		var attacker = info.Attacker;

		if ( attacker is Player && attacker != player )
			LogEvent( EventType.PlayerTookDamage, Game.Current.State.TimeLeft, $"{attacker.Client.Name} did {info.Damage} damage to {player.Client.Name}" );
		else
			LogEvent( EventType.PlayerTookDamage, Game.Current.State.TimeLeft, $"{player.Client.Name} took {info.Damage} damage." );
	}

	[TTTEvent.Player.Killed]
	private static void OnPlayerKilled( Player player )
	{
		if ( !Host.IsServer )
			return;

		if ( !player.DiedBySuicide )
			LogEvent( EventType.PlayerKill, Game.Current.State.TimeLeft, $"{player.LastAttacker.Client.Name} killed {player.Client.Name}" );
		else if ( player.LastDamageInfo.Flags == DamageFlags.Fall )
			LogEvent( EventType.PlayerSuicide, Game.Current.State.TimeLeft, $"{player.Client.Name} fell to their death." );
	}

	[TTTEvent.Player.CorpseFound]
	private static void OnCorpseFound( Player player )
	{
		if ( !Host.IsServer )
			return;

		LogEvent( EventType.PlayerCorpseFound, Game.Current.State.TimeLeft, $"{player.Confirmer.Client.Name} found the corpse of {player.Corpse.PlayerName}" );
	}

	private static void WriteEvents()
	{
		if ( !Game.LoggerEnabled )
			return;

		if ( !FileSystem.Data.DirectoryExists( LogFolder ) )
			FileSystem.Data.CreateDirectory( LogFolder );

		var mapFolderPath = $"{LogFolder}/{DateTime.Now:yyyy-MM-dd} {Global.MapName}";
		if ( !FileSystem.Data.DirectoryExists( mapFolderPath ) )
			FileSystem.Data.CreateDirectory( mapFolderPath );

		var logFilePath = $"{mapFolderPath}/{DateTime.Now:yyyy-MM-dd HH.mm.ss}.txt";
		FileSystem.Data.WriteAllText( logFilePath, GetEventSummary() );
	}

	private static string GetEventSummary()
	{
		var summary = $"{DateTime.Now:yyyy-MM-dd HH.mm.ss} - {Global.MapName}\n";

		for ( var i = 0; i < Events.Count; ++i )
			summary += $"{Events[i].Time.TimerString()} - {Events[i].Description}\n";

		return summary;
	}
}
