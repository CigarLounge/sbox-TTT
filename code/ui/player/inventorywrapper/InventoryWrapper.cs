using Sandbox.UI;

namespace TTT.UI;

public class InventoryWrapper : Panel
{
	public InventoryWrapper()
	{
		StyleSheet.Load( "/ui/player/inventorywrapper/InventoryWrapper.scss" );

		AddChild<PerkDisplay>();
		AddChild<InventorySelection>();
	}
}
