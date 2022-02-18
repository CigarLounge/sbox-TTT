namespace TTT.Roles;

public class InnocentRole : BaseRole
{
	public override Team Team => Team.Innocents;
	public override string Name => "Innocent";
	public override Color Color => Color.FromBytes( 27, 197, 78 );

	// serverside function
	public override void CreateDefaultShop()
	{
		base.CreateDefaultShop();
	}
}
