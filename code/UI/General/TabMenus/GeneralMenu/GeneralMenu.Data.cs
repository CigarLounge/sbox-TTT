using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

// TODO: Let's consider moving this somewhere else, or changing the structure.
public struct MenuData
{
	public Player[] Innocents { get; set; }
	public Player[] Detectives { get; set; }
	public Player[] Traitors { get; set; }
}

public partial class GeneralMenu : Panel
{
	// TODO: Do we wanna move this somewhere else?
	public MenuData Data;

	// TODO: Once we pass down some more data, let's change the way we determine this.
	public bool HasRoundData => !Data.Innocents.IsNullOrEmpty();

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
