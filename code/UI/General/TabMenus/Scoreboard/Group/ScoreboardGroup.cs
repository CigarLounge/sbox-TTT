using Sandbox;
using Sandbox.UI;

namespace TTT.UI;

[UseTemplate]
public class ScoreboardGroup : Panel
{
	public readonly PlayerStatus GroupStatus;
	public int GroupMembers = 0;

	private Label Title { get; init; }
	private Panel Content { get; init; }

	public ScoreboardGroup( Panel parent, PlayerStatus GroupStatus ) : base( parent )
	{
		this.GroupStatus = GroupStatus;
		Title.Text = GroupStatus.ToStringFast();
		AddClass( GroupStatus.ToStringFast() );
	}

	public ScoreboardEntry AddEntry( IClient client )
	{
		var scoreboardEntry = new ScoreboardEntry()
		{
			Parent = Content,
			Client = client,
			PlayerStatus = GroupStatus
		};

		return scoreboardEntry;
	}
}
