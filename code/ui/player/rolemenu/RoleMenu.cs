using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleMenu : Panel
{
	private Label RoleHeader { get; set; }
	private TabContainer TabContainer { get; set; }

	public RoleMenu()
	{
		AddClass( "text-shadow" );

		TabContainer.AddTab( new Shop(), "Shop", "shopping_cart" );
	}

	public override void Tick()
	{

	}
}
