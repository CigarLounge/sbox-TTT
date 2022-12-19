using System.Linq;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

public partial class EventSummary : Panel
{
	public static EventSummary Instance;

	private Panel Header { get; set; }
	private Panel Events { get; set; }

	public EventSummary() => Instance = this;

	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		Init();
	}

	public void Init()
	{
		Events.DeleteChildren();

		if ( GeneralMenu.Instance is not null && !GeneralMenu.Instance.LastEventSummaryData.Events.IsNullOrEmpty() )
			foreach ( var summaryEvent in GeneralMenu.Instance.LastEventSummaryData.Events )
				AddEvent( summaryEvent );

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
