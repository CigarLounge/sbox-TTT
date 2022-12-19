using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class PerkSlot : Panel
{
	private readonly Perk _perk;

	public PerkSlot( Perk perk ) => _perk = perk;
	protected override int BuildHash() => HashCode.Combine( _perk.SlotText );
}
