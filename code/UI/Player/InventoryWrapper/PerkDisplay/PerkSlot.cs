using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class PerkSlot : Panel
{
	public Perk Perk { get; set; }
	protected override int BuildHash() => HashCode.Combine( Perk.SlotText );
}
