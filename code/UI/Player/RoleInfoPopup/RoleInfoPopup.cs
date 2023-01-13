using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class RoleInfoPopup : Panel
{
	private static RoleInfoPopup Instance { get; set; }

	private TimeUntil _timeUntilDeletion = 10000; // Change back to 10.

	public RoleInfoPopup()
	{
		Instance?.Delete();
		Instance = this;
	}

	public override void Tick()
	{
		if ( _timeUntilDeletion )
		{
			Delete();
			Instance = null;
		}
	}

	[GameEvent.Round.Start]
	private static void OnRolesAssigned()
	{
		if ( !Game.IsClient )
			return;

		Game.RootPanel.AddChild( new RoleInfoPopup() );
	}
}
