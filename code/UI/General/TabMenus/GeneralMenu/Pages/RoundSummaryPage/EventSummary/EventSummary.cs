using System.Linq;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public class EventSummary : Panel
{
	public static EventSummary Instance;

	private Panel Empty { get; init; }
	private Panel Header { get; init; }
	private Panel Events { get; init; }

	public EventSummary()
	{
		Instance = this;
		Init();
	}

	public void Init()
	{
		Events.DeleteChildren();

		if ( GeneralMenu.Instance is not null )
			for ( var i = 0; i < GeneralMenu.Instance.LastEventSummaryData.Events.Length; ++i )
				AddEvent( GeneralMenu.Instance.LastEventSummaryData.Events[i] );

		Empty.Enabled( !Events.Children.Any() );
		Header.Enabled( Events.Children.Any() );
	}

	private void AddEvent( EventInfo eventInfo )
	{
		var container = Events.Add.Panel( "event" );
		container.Add.Label( GetIcon( eventInfo.EventType ), "icon" );
		container.Add.Label( eventInfo.Time.TimerString(), "time" );
		container.Add.Label( eventInfo.Description, "desc" );
	}

	private static string GetIcon( EventType eventType )
	{
		return eventType switch
		{
			EventType.Round => "flag",
			EventType.PlayerTookDamage => "error",
			EventType.PlayerKill => "group",
			EventType.PlayerSuicide => "person",
			EventType.PlayerCorpseFound => "search",
			_ => string.Empty,
		};
	}
}
