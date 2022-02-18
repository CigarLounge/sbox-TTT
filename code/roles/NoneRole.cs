namespace TTT.Roles;

public class NoneRole : BaseRole
{
	public override Color Color => Color.Transparent;

	// serverside function
	public override void CreateDefaultShop()
	{
		// Shop.Enabled = false;

		base.CreateDefaultShop();
	}
}
