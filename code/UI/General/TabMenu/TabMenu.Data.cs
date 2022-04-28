using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class TabMenu : Panel
{
	public Player[] Innocents { get; set; }
	public Player[] Detectives { get; set; }
	public Player[] Traitors { get; set; }

	[ClientRpc]
	public static void LoadPlayerData( Player[] innocents, Player[] detectives, Player[] traitors )
	{
		if ( Instance == null )
			return;

		Instance.Innocents = innocents;
		Instance.Detectives = detectives;
		Instance.Traitors = traitors;

		RoleSummary.Instance?.Init();
	}
}