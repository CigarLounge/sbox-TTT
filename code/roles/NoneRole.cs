using Sandbox;

namespace TTT;

[Library( "ttt_role_none" )]
public class NoneRole : BaseRole
{
	// serverside function
	public override void CreateDefaultShop()
	{
		// Shop.Enabled = false;

		base.CreateDefaultShop();
	}
}
