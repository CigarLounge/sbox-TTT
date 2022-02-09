using TTT.Teams;

namespace TTT.Roles
{
	[Role( "None" )]
	public class NoneRole : TTTRole
	{
		public override Color Color => Color.Transparent;

		public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam( typeof( NoneTeam ) );

		public override bool IsSelectable => false;

		public NoneRole() : base()
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
