using Sandbox.UI;

namespace TTT.UI;

public abstract class EntityHintPanel : Panel
{
}

public class HintDisplay : Panel
{
	public static HintDisplay Instance { get; set; }

	public HintDisplay() : base()
	{
		Instance = this;

		AddClass( "fullscreen" );
	}
}

[UseTemplate]
public class Hint : EntityHintPanel
{
	private Label HintLabel { get; set; }

	public Hint( string text )
	{
		HintLabel.Text = text;
		this.Enabled( false );
	}

	public void SetText( string text ) => HintLabel.Text = text;
}
