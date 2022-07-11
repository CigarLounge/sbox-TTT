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

	public ScoreboardEntry AddEntry( Client client )
	{
		var scoreboardEntry = new ScoreboardEntry( Content, client )
		{
			PlayerStatus = GroupStatus
		};

		scoreboardEntry.Update();

		return scoreboardEntry;
	}
}
