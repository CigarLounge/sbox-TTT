using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardGroup : Panel
{
	public readonly SomeState SomeState;
	public int GroupMembers = 0;

	private Label Title { get; init; }
	private Panel Content { get; init; }

	public ScoreboardGroup( Panel parent, SomeState someState ) : base( parent )
	{
		SomeState = someState;
		Title.Text = someState.ToString();
		AddClass( someState.ToString() );
	}

	public ScoreboardEntry AddEntry( Client client )
	{
		var scoreboardEntry = new ScoreboardEntry( Content, client )
		{
			SomeState = SomeState
		};

		scoreboardEntry.Update();

		return scoreboardEntry;
	}
}
