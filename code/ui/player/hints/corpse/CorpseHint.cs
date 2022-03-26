using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private readonly Corpse _corpse;
	private Label Title { get; set; }
	private Label SubText { get; set; }
	private InputGlyph TopButton { get; set; }
	private InputGlyph BottomButton { get; set; }
	private Panel ActionPanel { get; set; }
	private Panel CovertSearchPanel { get; set; }

	public CorpseHint() { }

	public CorpseHint( Corpse corpse ) => _corpse = corpse;

	public override void Tick()
	{
		base.Tick();

		bool isConfirmed = _corpse.DeadPlayer is not null && _corpse.DeadPlayer.IsConfirmedDead;

		Title.Text = _corpse.PlayerName ?? "Unidentified body";
		Title.Style.FontColor = _corpse.DeadPlayer?.Role.Color;
		Title.SetClass( "unidentified", !isConfirmed );
		SubText.Text = !isConfirmed ? "to identify" : "to search";

		if ( isConfirmed )
			CovertSearchPanel?.Delete();

		// We do not want to show the bottom "actions" panel if we are far away, or we are not currently using binoculars.
		if ( !_corpse.CanSearch() )
		{
			ActionPanel.Style.Opacity = 0;

			return;
		}

		var searchButton = Corpse.GetSearchButton();
		TopButton.SetButton( searchButton );
		BottomButton.SetButton( searchButton );

		ActionPanel.Style.Opacity = 100;
	}
}
