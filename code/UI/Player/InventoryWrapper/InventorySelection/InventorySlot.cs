using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class InventorySlot : Panel
{
	public Carriable Carriable { get; set; }
	public bool IsFirst { get; set; } = false;
	public bool IsLast { get; set; } = false;
	protected override int BuildHash()
	{
		return HashCode.Combine( Carriable.SlotText, CameraMode.Target.Role.Color, Carriable.IsActiveCarriable, IsFirst, IsLast );
	}
}
