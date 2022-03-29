using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class InventorySlot : Panel
{
	public Carriable Carriable { get; init; }
	public Label SlotLabel { get; set; }
	private Label SlotTitle { get; set; }
	private Label SlotText { get; set; }

	public InventorySlot( Panel parent, Carriable carriable ) : base( parent )
	{
		Parent = parent;
		Carriable = carriable;

		SlotLabel.Text = ((int)carriable.Info.Slot + 1).ToString();
		SlotTitle.Text = carriable.Info.Title;
		SlotText.Text = Carriable.SlotText;
	}

	public override void Tick()
	{
		base.Tick();

		var player = Local.Pawn as Player;
		SlotLabel.Style.BackgroundColor = player.CurrentPlayer.Role?.Color;
	}

	public void UpdateSlotText( string slotText )
	{
		SlotText.Text = slotText;
	}
}
