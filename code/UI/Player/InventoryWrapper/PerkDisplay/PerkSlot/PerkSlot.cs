using System;
using Sandbox.UI;

namespace TTT.UI;

public partial class PerkSlot : Panel
{
	private Image Icon { get; set; }
	private readonly Perk _perk;

	public PerkSlot( Perk perk ) => _perk = perk;
	protected override int BuildHash() => HashCode.Combine( _perk.SlotText );
	protected override void OnAfterTreeRender( bool firstTime )
	{
		if ( !firstTime )
			return;

		Icon.SetTexture( _perk.Info.Icon );
	}
}
