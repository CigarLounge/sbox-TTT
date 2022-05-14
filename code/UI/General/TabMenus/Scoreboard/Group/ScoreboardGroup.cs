using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardGroup : Panel
{
	public readonly string GroupTitle;
	public int GroupMembers = 0;

	private Label Title { get; init; }
	private Panel Content { get; init; }

	public ScoreboardGroup( Panel parent, string groupName ) : base( parent )
	{
		GroupTitle = groupName;
		Title.Text = groupName;
		AddClass( groupName );
	}

	public ScoreboardEntry AddEntry( Client client )
	{
		var scoreboardEntry = new ScoreboardEntry( Content, client )
		{
			ScoreboardGroupName = GroupTitle
		};

		scoreboardEntry.Update();

		return scoreboardEntry;
	}
}
