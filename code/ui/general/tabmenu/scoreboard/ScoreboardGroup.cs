using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public partial class ScoreboardGroup : Panel
{
	public readonly string GroupTitle;
	public int GroupMembers = 0;

	private Label Title { get; set; }
	private Panel Content { get; set; }

	public ScoreboardGroup( Panel parent, string groupName ) : base( parent )
	{
		GroupTitle = groupName;
		AddClass( groupName );
	}

	public ScoreboardEntry AddEntry( Client client )
	{
		ScoreboardEntry scoreboardEntry = Content.AddChild<ScoreboardEntry>();
		scoreboardEntry.ScoreboardGroupName = GroupTitle;
		scoreboardEntry.Client = client;
		scoreboardEntry.Update();
		return scoreboardEntry;
	}

	public void UpdateTitle()
	{
		Title.Text = GroupTitle;
	}
}
