using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTT.UI;

[UseTemplate]
public partial class InfoFeedEntry : Panel
{
	private readonly TimeUntil _timeUntilDelete = 6;

	public Label AddLabel( string text, string classname )
	{
		var label = Add.Label( text, classname );
		return label;
	}

	public override void Tick()
	{
		base.Tick();

		if ( _timeUntilDelete )
			Delete();
	}
}
