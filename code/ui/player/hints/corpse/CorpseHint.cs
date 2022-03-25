using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private readonly Corpse _corpse;
	private Label Title { get; set; }
	private InputGlyph TopButton { get; set; }
	private InputGlyph BottomButton { get; set; }
	private Panel ActionPanel { get; set; }
	private Panel CovertSearchPanel { get; set; }

	public CorpseHint() { }

	public CorpseHint( Corpse corpse ) => _corpse = corpse;

	public override void Tick()
	{
		base.Tick();

		if ( Local.Pawn is not Player player )
			return;

		TopButton.SetButton( Corpse.GetSearchButton() );
		BottomButton.SetButton( Corpse.GetSearchButton() );

		var isConfirmed = _corpse.DeadPlayer is not null && _corpse.DeadPlayer.IsConfirmedDead;
		Title.Text = !isConfirmed ? "Unidentified body" : _corpse.PlayerName;
		Title.SetClass( "unidentified", !isConfirmed );
		if ( isConfirmed )
			CovertSearchPanel?.Delete();

		if ( player.IsLookingAtHintableEntity( Player.USE_DISTANCE ) == null )
		{
			ActionPanel.Style.Opacity = 0;
			return;
		}

		ActionPanel.Style.Opacity = 100;
	}
}
