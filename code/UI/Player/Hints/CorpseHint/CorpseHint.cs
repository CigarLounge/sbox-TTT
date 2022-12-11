using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private readonly Corpse _corpse;

	private Label Title { get; init; }
	private Label SubText { get; init; }
	private Label CreditHint { get; init; }
	private InputGlyph TopButton { get; init; }
	private InputGlyph BottomButton { get; init; }
	private Panel ActionPanel { get; init; }
	private Panel CovertSearchPanel { get; init; }

	public CorpseHint( Corpse corpse ) => _corpse = corpse;

	public override void Tick()
	{
		base.Tick();

		if ( Game.LocalPawn is not Player player )
			return;

		Title.Text = _corpse.Player?.SteamName ?? "Unidentified body";
		Title.Style.FontColor = _corpse.Player?.Role.Color;
		Title.SetClass( "unidentified", _corpse.Player is null );

		if ( _corpse.Player is not null )
		{
			if ( _corpse.Player.IsConfirmedDead )
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

		var canFetchCredits = _corpse.HasCredits && player.Role.CanRetrieveCredits && player.IsAlive();
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
