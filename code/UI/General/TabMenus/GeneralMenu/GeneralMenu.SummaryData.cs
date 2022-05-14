using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class GeneralMenu : Panel
{
	public struct RoleSummaryData
	{
		public Player[] Innocents { get; set; }
		public Player[] Detectives { get; set; }
		public Player[] Traitors { get; set; }
	}

	public struct EventSummaryData
	{
		public EventInfo[] Events { get; set; }
		public string[] EventDescriptions { get; set; }
	}

	public RoleSummaryData LastRoleSummaryData;
	public EventSummaryData LastEventSummaryData;

	[ClientRpc]
	public static void SendRoleSummaryData( Player[] innocents, Player[] detectives, Player[] traitors )
	{
		if ( Instance is null )
			return;

		Instance.LastRoleSummaryData.Innocents = innocents;
		Instance.LastRoleSummaryData.Detectives = detectives;
		Instance.LastRoleSummaryData.Traitors = traitors;

		RoleSummary.Instance?.Init();
	}

	[ClientRpc]
	public static void SendEventSummaryData( EventInfo[] events, string[] eventDescriptions )
	{
		if ( Instance is null )
			return;

		Instance.LastEventSummaryData.Events = events;
		Instance.LastEventSummaryData.EventDescriptions = eventDescriptions;

		EventSummary.Instance?.Init();
	}
}
