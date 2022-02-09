using TTT.Teams;

namespace TTT.Roles
{
	[Role( "Innocent" )]
	public class InnocentRole : TTTRole
	{
		public override Color Color => Color.FromBytes( 27, 197, 78 );

		public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( InnocentTeam ) );

		public InnocentRole() : base()
		{

		}

		// serverside function
		public override void CreateDefaultShop()
		{
			Shop.Enabled = false;

			base.CreateDefaultShop();
		}
	}
}
