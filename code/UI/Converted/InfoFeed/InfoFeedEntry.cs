using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

public partial class InfoFeedEntry : Panel
{
	private readonly TimeUntil _timeUntilDelete = 6;

	public override void Tick()
	{
		base.Tick();

		if ( _timeUntilDelete )
			Delete();
	}
}
