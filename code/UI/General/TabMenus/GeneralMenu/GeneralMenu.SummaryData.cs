using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace TTT.UI;

public partial class GeneralMenu : Panel
{
	public struct RoleSummaryData
	{
		public List<Player> Innocents { get; set; }
		public List<Player> Detectives { get; set; }
		public List<Player> Traitors { get; set; }
	}

	public struct EventSummaryData
	{
		public EventInfo[] Events { get; set; }
	}

	public RoleSummaryData LastRoleSummaryData;
	public EventSummaryData LastEventSummaryData;

	[ClientRpc]
	public static void SendSummaryData( byte[] eventBytes )
	{
		if ( Instance is null )
			return;

		Instance.LastEventSummaryData.Events = EventInfo.Deserialize( eventBytes );
		EventSummary.Instance?.Init();

		Instance.LastRoleSummaryData.Innocents = Role.Innocent.Players.ToList();
		Instance.LastRoleSummaryData.Traitors = Role.Traitor.Players.ToList();
		Instance.LastRoleSummaryData.Detectives = Role.Detective.Players.ToList();	

		RoleSummary.Instance?.Init();
	}
}
