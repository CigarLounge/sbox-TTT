using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public struct MenuData
{
	public Player[] Innocents { get; set; }
	public Player[] Detectives { get; set; }
	public Player[] Traitors { get; set; }
}

public partial class GeneralMenu : Panel
{
	public MenuData Data;

	[ClientRpc]
	public static void LoadPlayerData( Player[] innocents, Player[] detectives, Player[] traitors )
	{
		if ( Instance == null )
			return;

		Instance.Data.Innocents = innocents;
		Instance.Data.Detectives = detectives;
		Instance.Data.Traitors = traitors;

		RoleSummary.Instance?.Init();
	}
}
