using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private readonly Player _searcher;
	private readonly Corpse _corpse;

	private Label Title { get; set; }
	private Label SubText { get; set; }
	private Label CreditHint { get; set; }
	private InputGlyph TopButton { get; set; }
	private InputGlyph BottomButton { get; set; }
	private Panel ActionPanel { get; set; }
	private Panel CovertSearchPanel { get; set; }

	public CorpseHint() { }

	public CorpseHint( Player searcher, Corpse corpse )
	{
		_searcher = searcher;
		_corpse = corpse;
	}

	public override void Tick()
	{
		base.Tick();

		Title.Text = _corpse.PlayerName ?? "Unidentified body";
		Title.Style.FontColor = _corpse.DeadPlayer?.Role.Color;
		Title.SetClass( "unidentified", _corpse.DeadPlayer is null );

		if ( _corpse.DeadPlayer is not null )
		{
			if ( _corpse.DeadPlayer.IsConfirmedDead )
			{
				SubText.Text = "to search.";
				CovertSearchPanel?.Delete();
			}
			else
			{
				SubText.Text = "to confirm.";
			}
		}
		else
		{
			SubText.Text = "to identify.";
		}

		var canFetchCredits = _corpse.Credits > 0 && _searcher.Role.CanRetrieveCredits && _searcher.IsAlive();
		if ( !canFetchCredits )
			CreditHint?.Delete();

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
