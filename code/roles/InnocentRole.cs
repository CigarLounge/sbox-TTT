using TTT.Teams;

namespace TTT.Roles;

public class InnocentRole : TTTRole
{
	public override string Name => "Innocent";
	public override Color Color => Color.FromBytes( 27, 197, 78 );
	public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( InnocentTeam ) );

	// serverside function
	public override void CreateDefaultShop()
	{
		Shop.Enabled = false;

		base.CreateDefaultShop();
	}
}
