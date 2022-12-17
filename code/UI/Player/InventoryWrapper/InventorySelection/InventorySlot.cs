using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class InventorySlot : Panel
{
	public Carriable Carriable { get; set; }
	public Panel SlotNumber { get; set; }
	protected override int BuildHash() => HashCode.Combine( Carriable.SlotText, PlayerCamera.Target.Role.Color );
}
