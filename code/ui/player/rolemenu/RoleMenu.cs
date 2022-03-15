using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class RoleMenu : Panel
{
	public static RoleMenu Instance;

	private Label RoleHeader { get; set; }
	private TabContainer TabContainer { get; set; }

	public RoleMenu()
	{
		Instance = this;

		AddClass( "text-shadow" );

		TabContainer.AddTab( new Shop(), "Shop", "shopping_cart" );

		// TODO: Remove
		AddRadioTab();
	}

	public void AddRadioTab()
	{
		TabContainer.AddTab( new RadioMenu(), "Radio", "radio" );
	}

	public override void Tick()
	{

	}
}
