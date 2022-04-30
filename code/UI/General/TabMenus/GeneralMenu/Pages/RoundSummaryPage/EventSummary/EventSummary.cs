using System.Linq;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public partial class EventSummary : Panel
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
		Events?.DeleteChildren();

		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );
		AddEvent( "flag", 523, "The round has started!" );

		Empty.Enabled( !Events.Children.Any() );
		Header.Enabled( Events.Children.Any() );
	}

	// TODO: Add proper hookups to event data...
	private void AddEvent( string eventType, float time, string description )
	{
		var container = Events.Add.Panel( "event" );
		container.Add.Label( GetIcon( eventType ), "icon" );
		container.Add.Label( time.TimerString(), "time" );
		container.Add.Label( description, "desc" );
	}

	// TODO: Proper switch statement.
	private string GetIcon( string eventType )
	{
		return eventType;
	}
}
