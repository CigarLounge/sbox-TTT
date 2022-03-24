using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private readonly Corpse _corpse;
	private Label Title { get; set; }
	private InputGlyph TopButton { get; set; }
	private InputGlyph BottomButton { get; set; }

	public CorpseHint() { }

	public CorpseHint( Corpse corpse ) => _corpse = corpse;

	public override void Tick()
	{
		base.Tick();

		TopButton.SetButton( Corpse.GetSearchButton() );
		BottomButton.SetButton( Corpse.GetSearchButton() );
		Title.Text = _corpse.PlayerName ?? "Unidentified body";
		Title.SetClass( "unidentified", _corpse.PlayerName is null );
	}
}
