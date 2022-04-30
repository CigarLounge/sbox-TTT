using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

// TODO: Let's consider moving this somewhere else, or changing the structure.
public struct MenuData
{
	public long[] InnocentClientIds { get; set; }
	public long[] DetectiveClientIds { get; set; }
	public long[] TraitorClientIds { get; set; }
}

public partial class GeneralMenu : Panel
{
	// TODO: Do we wanna move this somewhere else?
	public MenuData Data;

	// TODO: Once we pass down some more data, let's change the way we determine this.
	public bool HasRoundData => !Data.InnocentClientIds.IsNullOrEmpty();

	[ClientRpc]
	public static void LoadPlayerData( long[] innocents, long[] detectives, long[] traitors )
	{
		if ( Instance == null )
			return;

		Instance.Data.InnocentClientIds = innocents;
		Instance.Data.DetectiveClientIds = detectives;
		Instance.Data.TraitorClientIds = traitors;

		RoleSummary.Instance?.Init();
	}
}
