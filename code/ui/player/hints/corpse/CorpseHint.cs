using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class CorpseHint : EntityHintPanel
{
	private Label Title { get; set; }

	public CorpseHint()
	{
		this.Enabled( false );
	}

	public override void UpdateHintPanel( string playerName, string subtext = "" )
	{
		Title.Text = playerName ?? "Unidentified body";
		Title.SetClass( "unidentified", playerName == null );
	}
}
