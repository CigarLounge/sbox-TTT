using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	public Corpse Corpse { get; init; }
	private Label Title { get; set; }
	private InputGlyph TopButton { get; set; }
	private InputGlyph BottomButton { get; set; }

	public CorpseHint() { }

	public CorpseHint( Corpse corpse ) => Corpse = corpse;

	public override void Tick()
	{
		base.Tick();

		TopButton.SetButton( Corpse.GetSearchButton() );
		BottomButton.SetButton( Corpse.GetSearchButton() );
		Title.Text = Corpse.PlayerName ?? "Unidentified body";
		Title.SetClass( "unidentified", Corpse.PlayerName is null );
	}
}
