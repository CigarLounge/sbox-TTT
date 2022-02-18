using Sandbox.UI;

namespace TTT.UI;

public class InventoryWrapper : Panel
{
	public InventoryWrapper()
	{
		StyleSheet.Load( "/ui/generalhud/inventorywrapper/InventoryWrapper.scss" );

		AddChild<PerkDisplay>();
		AddChild<InventorySelection>();
	}
}
