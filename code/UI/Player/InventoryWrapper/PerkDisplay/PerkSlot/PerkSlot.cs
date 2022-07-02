using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class PerkSlot : Panel
{
	private readonly Perk _perk;
	private Image Icon { get; set; }
	private Label IconText { get; set; }
	private Label IconName { get; set; }

	public PerkSlot( Perk perk )
	{
		_perk = perk;
		Icon.SetTexture( perk.Info.CachedIcon );
		IconName.Text = perk.Info.Title;
	}

	public override void Tick()
	{
		base.Tick();

		IconText.Text = _perk.SlotText;
	}
}
